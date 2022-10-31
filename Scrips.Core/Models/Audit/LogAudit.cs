using System;

namespace Scrips.Core.Models.Audit
{
    public class LogAudit
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Tenant { get; set; }
        public Guid User { get; set; }
        public string Ip { get; set; }
        public string Entity { get; set; } //Name of Entity/Table ex: Organization
        public string Action { get; set; } //Added, Updated, Deleted
        public string KeyValues { get; set; } //Primary/Foreign keys of the record(as json)
        public string OldValues { get; set; } //Actual values before the change(as json)
        public string NewValues { get; set; } //Actual values after the change(as json)
    }
}
