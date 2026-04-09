using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Scrips.Core.Models.Audit;
using Serilog;

namespace Scrips.BaseDbContext;

public class AuditableBaseDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DaprClient _daprClient;
    private const int MaxRetries = 3;

    public AuditableBaseDbContext(DbContextOptions option, IHttpContextAccessor httpContextAccessor, DaprClient daprClient)
        : base(option)
    {
        _httpContextAccessor = httpContextAccessor;
        _daprClient = daprClient;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<LogAudit>? changes = null;
        try
        {
            changes = AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor);
            if (changes != null && changes.Any())
            {
                await SaveAudit(changes);
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Audit logging failed for {ChangeCount} entity changes — audit trail incomplete (compliance risk)", changes?.Count ?? 0);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        List<LogAudit>? changes = null;
        try
        {
            changes = AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor);
            if (changes != null && changes.Any())
            {
                var capturedChanges = changes;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SaveAudit(capturedChanges);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Background audit logging failed for {ChangeCount} entity changes — audit trail incomplete (compliance risk)", capturedChanges.Count);
                    }
                });
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
