﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.JsonWebTokens;
using Scrips.BaseDbContext.Entities;
using Scrips.Core.Models.Audit;
using System.Reflection;

namespace Scrips.BaseDbContext;

public class AuditLoggingHelper
{
    public static List<LogAudit> DetectChanges(ChangeTracker changeTracker, IHttpContextAccessor httpContextAccessor)
    {
        // Get all changed records excluding Audit records themselves
        var changes = changeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached && x.Entity is not LogAudit).ToList();

        // Get the user and other related identities, preferred to have an extension methods like GetUserId(), GetTenantId(), ...
        string? user = httpContextAccessor?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        string? tenant = httpContextAccessor?.HttpContext?.User?.FindFirst("tenant")?.Value;

        var ip = httpContextAccessor?.HttpContext?.Request.Headers["ipaddress"];

        // list of logs to be saved
        var logs = new List<LogAudit>();
        const string maskedValue = "*"; // default value to save instead of masked
        bool maskValue = false;

        foreach (var change in changes)
        {
            var entry = new AuditEntityEntry();
            if (user is not null) // will there be changes without user like signup!?
                entry.User = Guid.Parse(user);

            entry.Entity = change.Entity.GetType().Name;
            entry.Ip = ip;
            entry.Tenant = tenant;

            foreach (var prop in change.Properties)
            {
                maskValue = false;
                if (prop.Metadata.PropertyInfo != null && prop.Metadata.PropertyInfo.GetCustomAttribute<MaskValueAuditAttribute>() is not null)
                    maskValue = true;

                if (prop.Metadata.IsPrimaryKey())
                {
                    entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue ?? string.Empty;
                }
                else
                {
                    switch (change.State)
                    {
                        case EntityState.Deleted:
                            entry.AuditActionType = AuditActionType.Deleted;
                            entry.OldValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.OriginalValue ?? string.Empty;
                            break;
                        case EntityState.Added:
                            entry.AuditActionType = AuditActionType.Added;
                            entry.NewValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.CurrentValue ?? string.Empty;
                            break;
                        case EntityState.Modified:
                            if (prop.IsModified && ((prop.OriginalValue != null && !prop.OriginalValue.Equals(prop.CurrentValue))
                                                                || (prop.CurrentValue != null && !prop.CurrentValue.Equals(prop.OriginalValue))))
                            {
                                entry.AuditActionType = AuditActionType.Updated;
                                entry.OldValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.OriginalValue ?? string.Empty;
                                entry.NewValues[prop.Metadata.Name] = maskValue ? maskedValue : prop.CurrentValue ?? string.Empty;
                            }

                            break;
                        case EntityState.Detached:
                            break;
                        case EntityState.Unchanged:
                            break;
                    }
                }
            }

            if (entry.AuditActionType == 0)
                continue;

            logs.Add(entry.ToLogAudit());
        }

        return logs;
    }
}
