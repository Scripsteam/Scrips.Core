using Dapper;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Scrips.BaseDbContext.Dtos;
using Scrips.BaseDbContext.Entities;
using Serilog;
using System.Data;

namespace Scrips.BaseDbContext
{
    public class AuditableBaseDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _connectionSting;
        public AuditableBaseDbContext()
        { }

        public AuditableBaseDbContext(DbContextOptions option, IHttpContextAccessor httpContextAccessor, string connectionString) : base(option)
        {
            _httpContextAccessor = httpContextAccessor;
            _connectionSting = connectionString;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var changes = DetectChanges();
                if (changes != null && changes.Any())
                {
                    SaveAudit(changes);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            try
            {
                var changes = DetectChanges();
                if (changes != null && changes.Any())
                {
                    SaveAudit(changes);
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
            var user = _httpContextAccessor?.HttpContext?.User?.FindFirst(JwtClaimTypes.Subject)?.Value;
            var tenant = _httpContextAccessor?.HttpContext?.User?.FindFirst("tenant")?.Value;

            string ip = _httpContextAccessor?.HttpContext.Request.Headers["ipaddress"];

            //list of logs to be saved
            var logs = new List<LogAudit>();

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
                    if (prop.Metadata.IsPrimaryKey())
                        entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    else switch (change.State)
                        {
                            case EntityState.Deleted:
                                entry.AuditActionType = AuditActionType.Deleted;
                                entry.OldValues[prop.Metadata.Name] = prop.OriginalValue;
                                break;
                            case EntityState.Added:
                                entry.AuditActionType = AuditActionType.Added;
                                entry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                                break;
                            case EntityState.Modified:
                                if (prop.IsModified)
                                {
                                    entry.AuditActionType = AuditActionType.Updated;
                                    entry.OldValues[prop.Metadata.Name] = prop.OriginalValue;
                                    entry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                                }
                                break;
                        }
                }
                logs.Add(entry.ToLogAudit());
            }
            return logs;
        }

        private void SaveAudit(List<LogAudit> changes)
        {
            string sql = "";
            foreach (var change in changes)
            {
                sql = sql + string.Format(@"INSERT INTO[dbo].[LogAudit]([Timestamp],[Tenant],[User],[Ip],[Entity],[Action],[KeyValue],[OldValue],[NewValue])
                             VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", DateTime.UtcNow, change.Tenant, change.User, change.Ip, change.Entity, change.Action, change.KeyValues, change.OldValues, change.NewValues);
            }

            using IDbConnection connection = new SqlConnection(_connectionSting);
            connection.Query(sql);
        }
    }
}