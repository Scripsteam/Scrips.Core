using Dapr.Client;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Scrips.Core.Models.Audit;
using Scrips.BaseDbContext.Outbox;
using Serilog;

namespace Scrips.BaseDbContext;

public class AuditableMultiTenantBaseDbContext : MultiTenantDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DaprClient _daprClient;
    private readonly string? _tenantId;
    private const int MaxRetries = 3;

    public AuditableMultiTenantBaseDbContext(ITenantInfo tenantInfo, IHttpContextAccessor httpContextAccessor, DaprClient daprClient)
        : base(tenantInfo)
    {
        _httpContextAccessor = httpContextAccessor;
        _daprClient = daprClient;
        _tenantId = tenantInfo?.Id;
    }

    public AuditableMultiTenantBaseDbContext(ITenantInfo tenantInfo, DbContextOptions option, IHttpContextAccessor httpContextAccessor, DaprClient daprClient)
        : base(tenantInfo, option)
    {
        _httpContextAccessor = httpContextAccessor;
        _daprClient = daprClient;
        _tenantId = tenantInfo?.Id;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OutboxModelConfig.Apply(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<LogAudit>? changes = null;
        try
        {
            changes = AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor);
            if (changes != null && changes.Any())
            {
                if (AuditOutboxRuntime.Enabled)
                    Set<OutboxMessage>().Add(OutboxMessage.ForAudit(changes, _tenantId)); // committed atomically by base.SaveChangesAsync below
                else
                    await SaveAudit(changes);
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Audit logging failed for {ChangeCount} entity changes — audit trail incomplete (compliance risk)", changes?.Count ?? 0);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    // Intentional sync-over-async: DaprClient.PublishEventAsync is the only API available
    // for publishing audit events, and SaveChanges() must remain synchronous per the
    // DbContext contract. Task.Run avoids deadlocking the calling SynchronizationContext.
    // This is required for NABIDH audit trail compliance — every entity change must be
    // published before the save completes. Reviewed and approved in SND-331 / SND-386.
    public override int SaveChanges()
    {
        List<LogAudit>? changes = null;
        try
        {
            changes = AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor);
            if (changes != null && changes.Any())
            {
                if (AuditOutboxRuntime.Enabled)
                    Set<OutboxMessage>().Add(OutboxMessage.ForAudit(changes, _tenantId)); // committed atomically by base.SaveChanges below
                else
                    Task.Run(() => SaveAudit(changes)).GetAwaiter().GetResult();
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Audit logging failed for {ChangeCount} entity changes — audit trail incomplete (compliance risk)", changes?.Count ?? 0);
        }

        return base.SaveChanges();
    }

    private async Task SaveAudit(List<LogAudit> changes)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await _daprClient.PublishEventAsync("pubsub", "SaveAudit", changes);
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                Log.Warning("Dapr publish attempt {Attempt}/{MaxRetries} failed: {ErrorMessage}", attempt, MaxRetries, ex.Message);

                if (attempt < MaxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)));
                }
            }
        }

        // All retries exhausted — log the audit payload so it can be recovered from log aggregation
        Log.Error(lastException,
            "Audit publish failed after {MaxRetries} retries. Logging audit payload for recovery. AuditPayload={AuditPayload}",
            MaxRetries, JsonConvert.SerializeObject(changes));
    }
}
