using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Scrips.Core.Models.Audit;
using Serilog;

namespace Scrips.BaseDbContext;

public class AuditableBaseDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DaprClient _daprClient;

    public AuditableBaseDbContext(DbContextOptions option, IHttpContextAccessor httpContextAccessor, DaprClient daprClient) : base(option)
    {
        _httpContextAccessor = httpContextAccessor;
        _daprClient = daprClient;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var changes = AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor);
            if (changes != null && changes.Any())
            {
                await SaveAudit(changes);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        try
        {
            var changes = AuditLoggingHelper.DetectChanges(ChangeTracker, _httpContextAccessor);
            if (changes != null && changes.Any())
            {
                _ = SaveAudit(changes).Result;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        return base.SaveChanges();
    }

    private async Task<bool> SaveAudit(List<LogAudit> changes)
    {

        if (await _daprClient.CheckHealthAsync())
            await _daprClient.PublishEventAsync("pubsub", "SaveAudit", changes);
        return true;
    }
}