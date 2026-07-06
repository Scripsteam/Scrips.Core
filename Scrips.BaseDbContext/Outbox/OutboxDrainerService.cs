using System.Data;
using System.Data.Common;
using System.Text.Json;
using Dapr.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scrips.Core.Models.Audit;

namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Drains the OutboxMessages table to Dapr on a background thread — where the retry/backoff
/// belongs, off the request path. Pod-safe without leader election: the batch claim uses
/// ROWLOCK + READPAST so N pods drain in parallel with no double-publish, and a visibility-timeout
/// reaper reclaims rows a crashed pod left Claimed. Generic over the service's DbContext type.
/// </summary>
public class OutboxDrainerService<TContext> : BackgroundService where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AuditOutboxOptions _opts;
    private readonly ILogger<OutboxDrainerService<TContext>> _logger;

    public OutboxDrainerService(IServiceScopeFactory scopeFactory,
        IOptions<AuditOutboxOptions> opts, ILogger<OutboxDrainerService<TContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _opts = opts.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(1, _opts.PollSeconds));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
                var dapr = scope.ServiceProvider.GetRequiredService<DaprClient>();

                var conn = ctx.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(stoppingToken);

                await ReapStaleClaimsAsync(conn, stoppingToken);
                await DrainBatchAsync(conn, dapr, stoppingToken);
                await PurgePublishedAsync(conn, stoppingToken);
            }
            catch (Exception ex)
            {
                // Never let the loop die — a transient DB/broker error just retries next tick.
                _logger.LogError(ex, "Outbox drain iteration failed for {Context}", typeof(TContext).Name);
            }

            try { await Task.Delay(interval, stoppingToken); }
            catch (TaskCanceledException) { break; }
        }
    }

    /// <summary>Visibility timeout: reclaim rows a dead pod left Claimed. Resets state only — never touches Attempts.</summary>
    private async Task ReapStaleClaimsAsync(DbConnection conn, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
UPDATE [{OutboxModelConfig.TableName}] WITH (ROWLOCK, READPAST)
   SET Status = {OutboxStatus.Pending}, ClaimedUtc = NULL
 WHERE Status = {OutboxStatus.Claimed}
   AND ClaimedUtc < DATEADD(minute, -@vt, SYSUTCDATETIME());";
        AddParam(cmd, "@vt", Math.Max(1, _opts.VisibilityTimeoutMinutes));
        var reclaimed = await cmd.ExecuteNonQueryAsync(ct);
        if (reclaimed > 0)
            _logger.LogWarning("Outbox reaper reclaimed {Count} stale Claimed rows ({Context}) — pod churn or short timeout", reclaimed, typeof(TContext).Name);
    }

    /// <summary>Retention (PROD-1479): one bounded DELETE per tick of Published rows past the retention window,
    /// so the table + PHI-at-rest footprint stays bounded. DeadLetter rows are deliberately NOT purged.</summary>
    private async Task PurgePublishedAsync(DbConnection conn, CancellationToken ct)
    {
        if (_opts.PublishedRetentionDays <= 0) return; // disabled

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
DELETE TOP (@n) FROM [{OutboxModelConfig.TableName}]
 WHERE Status = {OutboxStatus.Published}
   AND ProcessedUtc < DATEADD(day, -@days, SYSUTCDATETIME());";
        AddParam(cmd, "@n", Math.Max(1, _opts.PurgeBatchSize));
        AddParam(cmd, "@days", _opts.PublishedRetentionDays);
        var purged = await cmd.ExecuteNonQueryAsync(ct);
        if (purged > 0)
            _logger.LogDebug("Outbox retention purged {Count} Published rows ({Context})", purged, typeof(TContext).Name);
    }

    private async Task DrainBatchAsync(DbConnection conn, DaprClient dapr, CancellationToken ct)
    {
        var claimed = new List<(long Id, Guid EventId, string? TenantId, string Topic, string Payload)>();

        using (var claim = conn.CreateCommand())
        {
            // Claim increments ClaimCount, NOT Attempts — a crash/reclaim must never burn dead-letter budget.
            claim.CommandText = $@"
UPDATE TOP (@batch) [{OutboxModelConfig.TableName}] WITH (ROWLOCK, READPAST)
   SET Status = {OutboxStatus.Claimed}, ClaimedUtc = SYSUTCDATETIME(), ClaimCount = ClaimCount + 1
OUTPUT inserted.Id, inserted.EventId, inserted.TenantId, inserted.Topic, inserted.Payload
 WHERE Status = {OutboxStatus.Pending};";
            AddParam(claim, "@batch", Math.Max(1, _opts.BatchSize));
            using var reader = await claim.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                claimed.Add((
                    reader.GetInt64(0),
                    reader.GetGuid(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)));
            }
        }

        foreach (var row in claimed)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                // Per-publish timeout: a hung sidecar must not stall the whole drain loop.
                using var pubCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                pubCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(1, _opts.PublishTimeoutSeconds)));

                var changes = JsonSerializer.Deserialize<List<LogAudit>>(row.Payload) ?? new List<LogAudit>();
                await dapr.PublishEventAsync(_opts.PubSubName, row.Topic, changes, pubCts.Token);
                await MarkPublishedAsync(conn, row.Id, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break; // shutdown — leave the row Claimed for the reaper; not a publish failure
            }
            catch (Exception ex)
            {
                var (status, attempts) = await MarkFailedAsync(conn, row.Id, ct);
                if (status == OutboxStatus.DeadLetter)
                    _logger.LogError(ex,
                        "Outbox DEAD-LETTER: audit event {EventId} (tenant {TenantId}) dead-lettered after {Attempts} publish attempts ({Context}) — audit-completeness gap, needs recovery",
                        row.EventId, row.TenantId ?? "(none)", attempts, typeof(TContext).Name);
                else
                    _logger.LogWarning(ex,
                        "Outbox publish failed for event {EventId} (attempt {Attempts}, {Context}); will retry",
                        row.EventId, attempts, typeof(TContext).Name);
            }
        }
    }

    private async Task MarkPublishedAsync(DbConnection conn, long id, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"UPDATE [{OutboxModelConfig.TableName}]
   SET Status = {OutboxStatus.Published}, ProcessedUtc = SYSUTCDATETIME() WHERE Id = @id;";
        AddParam(cmd, "@id", id);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    /// <summary>Only a real publish failure counts toward DeadLetter, so pod churn can't silently drop audit.
    /// Returns the resulting (Status, Attempts) so the caller can log a dead-letter loudly.</summary>
    private async Task<(byte Status, int Attempts)> MarkFailedAsync(DbConnection conn, long id, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"UPDATE [{OutboxModelConfig.TableName}]
   SET Attempts = Attempts + 1,
       Status = CASE WHEN Attempts + 1 >= @max THEN {OutboxStatus.DeadLetter} ELSE {OutboxStatus.Pending} END,
       ClaimedUtc = NULL
OUTPUT inserted.Status, inserted.Attempts
 WHERE Id = @id;";
        AddParam(cmd, "@id", id);
        AddParam(cmd, "@max", Math.Max(1, _opts.MaxAttempts));
        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
            return (reader.GetByte(0), reader.GetInt32(1));
        return (OutboxStatus.Pending, 0);
    }

    private static void AddParam(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }
}
