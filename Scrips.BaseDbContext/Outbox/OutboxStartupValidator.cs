using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Fail-fast guard: when the outbox writer is enabled, refuse to start if
/// (a) the service's <typeparamref name="TContext"/> hasn't mapped <see cref="OutboxMessage"/>
/// (overrides OnModelCreating without calling <c>base.OnModelCreating</c>), or
/// (b) the <c>OutboxMessages</c> table / its required columns don't exist in the database.
/// Without this, the flag flips and audit events are silently dropped, or the drainer/recovery
/// pass throws at runtime (e.g. the 02:00 recovery pass) when a column is missing — both
/// unacceptable for an audit trail. Registered before the drainer so it throws during startup.
/// </summary>
public class OutboxStartupValidator<TContext> : IHostedService where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxStartupValidator(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    /// <summary>Columns the drainer/recovery raw SQL depends on — checked so a premature flag flip
    /// (before OutboxMessages.sql / the RecoveryAttempts+ScrubbedUtc ALTER runs) fails at boot, not at runtime.</summary>
    private static readonly string[] RequiredColumns =
    {
        "Id", "EventId", "TenantId", "Topic", "Payload", "CreatedUtc", "Status",
        "Attempts", "ClaimCount", "ClaimedUtc", "ProcessedUtc", "RecoveryAttempts", "ScrubbedUtc"
    };

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<TContext>();

        if (ctx.Model.FindEntityType(typeof(OutboxMessage)) is null)
        {
            throw new InvalidOperationException(
                $"Audit:UseOutbox is ON but {nameof(OutboxMessage)} is not mapped in {typeof(TContext).Name}. " +
                $"The service DbContext must call base.OnModelCreating(modelBuilder). Refusing to start — " +
                $"otherwise audit events would be silently dropped.");
        }

        var conn = ctx.Database.GetDbConnection();
        var openedHere = conn.State != System.Data.ConnectionState.Open;
        if (openedHere) await conn.OpenAsync(cancellationToken);
        try
        {
            var present = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('dbo.{OutboxModelConfig.TableName}');";
                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                    present.Add(reader.GetString(0));
            }

            if (present.Count == 0)
                throw new InvalidOperationException(
                    $"Audit:UseOutbox is ON but table [dbo.{OutboxModelConfig.TableName}] does not exist in " +
                    $"{typeof(TContext).Name}'s database. Run OutboxMessages.sql before enabling. Refusing to start.");

            var missing = RequiredColumns.Where(c => !present.Contains(c)).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException(
                    $"Audit:UseOutbox is ON but [dbo.{OutboxModelConfig.TableName}] is missing column(s): " +
                    $"{string.Join(", ", missing)}. Run the schema migration (incl. RecoveryAttempts/ScrubbedUtc) " +
                    $"before enabling. Refusing to start.");
        }
        finally
        {
            if (openedHere) await conn.CloseAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
