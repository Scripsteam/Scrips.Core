﻿using Newtonsoft.Json;
using Scrips.BaseDbContext.Dtos;

namespace Scrips.BaseDbContext.Entities
{
    public enum AuditActionType
    {
        Added = 1,
        Updated = 2,
        Deleted = 3
    }

    public class AuditEntityEntry
    {
        public Guid User { get; set; }
        public string Entity { get; set; }
        public AuditActionType AuditActionType { get; set; }
        public string Ip { get; set; }
        public string Tenant { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public LogAudit ToLogAudit()
        {
            var log = new LogAudit();
            log.Timestamp = DateTime.Now;
            log.User = User;
            log.Ip = Ip;
            log.Tenant = Tenant;
            log.Entity = Entity;
            log.Action = AuditActionType.ToString();
            log.KeyValues = JsonConvert.SerializeObject(KeyValues);
            log.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            log.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            return log;
        }

    }
}