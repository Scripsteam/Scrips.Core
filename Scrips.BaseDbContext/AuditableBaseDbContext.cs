using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Scrips.BaseDbContext.Entities;
using Scrips.Core.Models.Audit;
using Serilog;
using System.Reflection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Scrips.BaseDbContext
{
    public class AuditableBaseDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DaprClient _daprClient;

        public AuditableBaseDbContext()
        { }

        public AuditableBaseDbContext(DbContextOptions option, IHttpContextAccessor httpContextAccessor, DaprClient daprClient) : base(option)
        {
            _httpContextAccessor = httpContextAccessor;
            _daprClient = daprClient;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var changes = DetectChanges();
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
                var changes = DetectChanges();
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

        private List<LogAudit> DetectChanges()
        {
            //Get all changed records excluding Audit records themselves
            var changes = ChangeTracker.Entries()
                    .Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached && x.Entity is not LogAudit).ToList();

            //Get the user and other related identities, preferred to have an extension methods like GetUserId(), GetTenantId(), ...
            var user = _httpContextAccessor?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var tenant = _httpContextAccessor?.HttpContext?.User?.FindFirst("tenant")?.Value;

            string ip = _httpContextAccessor?.HttpContext.Request.Headers["ipaddress"];

            //list of logs to be saved
            var logs = new List<LogAudit>();
            var maskedValue = "*";//default value to save instead of masked
            var maskValue = false;

            foreach (var change in changes)
            {
                var entry = new AuditEntityEntry();
                if (user is not null)//will there be changes without user like signup!?
                    entry.User = Guid.Parse(user);
                entry.Entity = change.Entity.GetType().Name;
                entry.Ip = ip;
                entry.Tenant = tenant;

                foreach (var prop in change.Properties)
                {
                    maskValue = false;
                    if (prop.Metadata.PropertyInfo.GetCustomAttribute<MaskValueAuditAttribute>() is not null)
                        maskValue = true;

                    if (prop.Metadata.IsPrimaryKey())
                        entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    else switch (change.State)
                        {
                            case EntityState.Deleted:
                                entry.AuditActionType = AuditActionType.Deleted;
                                entry.OldValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.OriginalValue;
                                break;
                            case EntityState.Added:
                                entry.AuditActionType = AuditActionType.Added;
                                entry.NewValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.CurrentValue;
                                break;
                            case EntityState.Modified:
                                if (prop.IsModified && (prop.OriginalValue != null && !prop.OriginalValue.Equals(prop.CurrentValue)
                                                                    || (prop.CurrentValue != null && !prop.CurrentValue.Equals(prop.OriginalValue))))
                                {
                                    entry.AuditActionType = AuditActionType.Updated;
                                    entry.OldValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.OriginalValue;
                                    entry.NewValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.CurrentValue;
                                }
                                break;
                        }
                }
                logs.Add(entry.ToLogAudit());
            }
            return logs;
        }

        private async Task<bool> SaveAudit(List<LogAudit> changes)
        {

            if (await _daprClient.CheckHealthAsync())
                await _daprClient.PublishEventAsync("pubsub", "SaveAudit", changes);
            return true;
        }
    }
}