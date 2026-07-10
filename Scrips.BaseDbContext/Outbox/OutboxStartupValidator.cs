using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Fail-fast guard: when the outbox writer is enabled, refuse to start if the <c>OutboxMessages</c>
/// table / its required columns don't exist — so a premature flag flip (before the schema is applied)
/// fails at boot, not at the 02:00 recovery pass. The schema check uses the connection string from the
/// registered <see cref="DbContextOptions{TContext}"/> and does NOT activate the context, so it works
/// for multi-tenant (Finbuckle) contexts too (which can't be constructed outside a request scope —
/// no ambient tenant). The model-mapping check is best-effort: skipped (with a warning) when the
/// context can't be constructed at startup, since the schema check is the real guarantee and the
/// outbox row is written in the request scope where the tenant is present. Registered before the drainer.
/// </summary>
public class OutboxStartupValidator<TContext> : IHostedService where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxStartupValidator<TContext>> _logger;

    public OutboxStartupValidator(IServiceScopeFactory scopeFactory, ILogger<OutboxStartupValidator<TContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

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

        // (1) Schema check — authoritative. Connection string comes from the registered options; the
        // context is NOT activated, so this works for multi-tenant contexts (no tenant needed here).
        var connStr = OutboxConnectionResolver.Resolve<TContext>(scope.ServiceProvider);
        await using (var conn = new SqlConnection(connStr))
        {
            await conn.OpenAsync(cancellationToken);
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

        // (2) Model-mapping check — best effort. A multi-tenant (Finbuckle) context can't be built at
        // startup (no ambient tenant); skip rather than crash. The schema check above is the guarantee.
        TContext ctx;
        try
        {
            ctx = scope.ServiceProvider.GetRequiredService<TContext>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Outbox startup model-check skipped for {Context}: context could not be constructed at startup " +
                "(expected for multi-tenant contexts). Schema check passed — relying on it.", typeof(TContext).Name);
            return;
        }

        if (ctx.Model.FindEntityType(typeof(OutboxMessage)) is null)
            throw new InvalidOperationException(
                $"Audit:UseOutbox is ON but {nameof(OutboxMessage)} is not mapped in {typeof(TContext).Name}. " +
                "The service DbContext must call base.OnModelCreating(modelBuilder). Refusing to start.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
