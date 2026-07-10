using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using Dapr.Client;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

    /// <summary>Last day (UTC) the recovery pass ran — gates it to once per day at the configured hour.</summary>
    private DateOnly _lastRecoveryDay;

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
                var dapr = scope.ServiceProvider.GetRequiredService<DaprClient>();

                // Get the connection from the registered options WITHOUT activating the context.
                // A multi-tenant (Finbuckle) context can't be constructed in a background scope
                // (no ambient tenant → tenant factory returns null); the outbox SQL is all raw ADO,
                // so a bare connection is all the drainer needs.
                await using var conn = new SqlConnection(OutboxConnectionResolver.Resolve<TContext>(scope.ServiceProvider));
                await conn.OpenAsync(stoppingToken);

                await ReapStaleClaimsAsync(conn, stoppingToken);
                await DrainBatchAsync(conn, dapr, stoppingToken);
                await PurgePublishedAsync(conn, stoppingToken);
                if (_opts.RecoveryEnabled)
                    await MaybeRunRecoveryAsync(conn, stoppingToken);
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

    /// <summary>Hard cap on rows deleted per purge pass — protects prod from a misconfigured PurgeBatchSize.</summary>
    private const int MaxPurgeBatch = 5000;

    /// <summary>Retention (PROD-1479): one bounded DELETE per tick of Published rows past the retention window,
    /// so the table + PHI-at-rest footprint stays bounded. DeadLetter rows are deliberately NOT purged (retention policy: PROD-1481).</summary>
    private async Task PurgePublishedAsync(DbConnection conn, CancellationToken ct)
    {
        if (_opts.PublishedRetentionDays <= 0) return; // disabled

        using var cmd = conn.CreateCommand();
        // COALESCE(ProcessedUtc, CreatedUtc): a Published row that somehow has a null ProcessedUtc
        // (race/bug) still purges by age instead of leaking PHI forever. Batch is hard-capped so a
        // misconfigured PurgeBatchSize can't turn this into a large blocking DELETE.
        cmd.CommandText = $@"
DELETE TOP (@n) FROM [{OutboxModelConfig.TableName}]
 WHERE Status = {OutboxStatus.Published}
   AND COALESCE(ProcessedUtc, CreatedUtc) < DATEADD(day, -@days, SYSUTCDATETIME());";
        AddParam(cmd, "@n", Math.Clamp(_opts.PurgeBatchSize, 1, MaxPurgeBatch));
        AddParam(cmd, "@days", _opts.PublishedRetentionDays);
        var purged = await cmd.ExecuteNonQueryAsync(ct);
        if (purged > 0)
            _logger.LogDebug("Outbox retention purged {Count} Published rows ({Context})", purged, typeof(TContext).Name);
    }

    /// <summary>Once-per-day gate for the DeadLetter recovery + scrub passes at the configured off-peak hour.</summary>
    private async Task MaybeRunRecoveryAsync(DbConnection conn, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        if (now.Hour != _opts.RecoveryRunAtUtcHour || _lastRecoveryDay == today) return;

        var requeued = await RecoverDeadLettersAsync(conn, ct);
        var scrubbed = _opts.ScrubTerminalPayload ? await ScrubTerminalDeadLettersAsync(conn, ct) : 0;
        _lastRecoveryDay = today; // set after success — a throw retries on the next tick within the hour
        if (requeued > 0 || scrubbed > 0)
            _logger.LogInformation("Outbox recovery pass ({Context}): re-queued {Requeued}, scrubbed {Scrubbed} terminal-poison",
                typeof(TContext).Name, requeued, scrubbed);
    }

    /// <summary>Scheduled recovery: re-queue eligible DeadLetter rows so the normal drainer re-publishes them
    /// (transient outages self-heal into LogAudit). Resets Attempts to 0 (fresh publish budget) and bumps
    /// RecoveryAttempts (the cycle cap). Re-publish only — it never writes the audit DB directly, so per-service
    /// DB isolation holds.</summary>
    private async Task<int> RecoverDeadLettersAsync(DbConnection conn, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
UPDATE TOP (@batch) [{OutboxModelConfig.TableName}] WITH (ROWLOCK, READPAST)
   SET Status = {OutboxStatus.Pending}, Attempts = 0, RecoveryAttempts = RecoveryAttempts + 1, ClaimedUtc = NULL
 WHERE Status = {OutboxStatus.DeadLetter} AND RecoveryAttempts < @rmax;";
        AddParam(cmd, "@batch", Math.Max(1, _opts.RecoveryBatchSize));
        AddParam(cmd, "@rmax", Math.Max(1, _opts.RecoveryMaxAttempts));
        var n = await cmd.ExecuteNonQueryAsync(ct);
        if (n > 0)
            _logger.LogWarning("Outbox recovery re-queued {Count} dead-lettered audit events for re-publish ({Context})", n, typeof(TContext).Name);
        return n;
    }

    /// <summary>Terminal-poison rows (exhausted RecoveryMaxAttempts) are NEVER deleted — health-data retention
    /// duty (PROD-1481 / UAE Federal Law 2/2019). Their clinical Payload is scrubbed to an empty change-list,
    /// keeping the metadata row (EventId/TenantId/Topic/Attempts/CreatedUtc) as the audit-completeness record.</summary>
    private async Task<int> ScrubTerminalDeadLettersAsync(DbConnection conn, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
UPDATE TOP (@batch) [{OutboxModelConfig.TableName}] WITH (ROWLOCK)
   SET Payload = @scrub, ScrubbedUtc = SYSUTCDATETIME()
 WHERE Status = {OutboxStatus.DeadLetter} AND RecoveryAttempts >= @rmax AND ScrubbedUtc IS NULL;";
        AddParam(cmd, "@batch", Math.Max(1, _opts.RecoveryBatchSize));
        AddParam(cmd, "@rmax", Math.Max(1, _opts.RecoveryMaxAttempts));
        AddParam(cmd, "@scrub", OutboxMessage.ScrubbedPayload);
        var n = await cmd.ExecuteNonQueryAsync(ct);
        if (n > 0)
            _logger.LogWarning("Outbox scrubbed {Count} terminal-poison DeadLetter payloads to metadata-only ({Context})", n, typeof(TContext).Name);
        return n;
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
                // Carry the outbox EventId as the CloudEvent id so the consumer can dedupe
                // (insert-if-not-exists on LogAudit). Makes at-least-once redelivery — the reaper
                // retry AND the scheduled recovery re-queue — idempotent. Additive: the data payload
                // is unchanged (still List<LogAudit>), so it's backward-compatible with the consumer.
                var pubMeta = new Dictionary<string, string> { ["cloudevent.id"] = row.EventId.ToString() };
                await dapr.PublishEventAsync(_opts.PubSubName, row.Topic, changes, pubMeta, pubCts.Token);
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

/// <summary>
/// Resolves the outbox DB connection string from the registered <see cref="DbContextOptions{TContext}"/>
/// WITHOUT constructing the context — so the background drainer/validator work for multi-tenant
/// (Finbuckle) contexts too, which can't be activated outside a request scope (no ambient tenant).
/// The outbox table lives in the context's single (shared) database; all outbox SQL is raw ADO.
/// </summary>
internal static class OutboxConnectionResolver
{
    public static string Resolve<TCtx>(IServiceProvider sp) where TCtx : DbContext
    {
        var options = sp.GetRequiredService<DbContextOptions<TCtx>>();
        var cs = options.Extensions
            .OfType<RelationalOptionsExtension>()
            .Select(e => e.ConnectionString)
            .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException(
                $"Outbox: could not resolve a connection string from DbContextOptions<{typeof(TCtx).Name}>. " +
                "Register the context with UseSqlServer(connectionString) so the outbox drainer can reach the DB.");
        return cs!;
    }
}
