using Newtonsoft.Json;
using Scrips.Core.Models.Audit;

namespace Scrips.BaseDbContext.Entities;

public enum AuditActionType
{
    Added = 1,
    Updated = 2,
    Deleted = 3
}

public class AuditEntityEntry
{
    public Guid User { get; set; }

    /// <summary>
    /// The subject claim when it is not a GUID — e.g. a service/client principal on a
    /// machine-to-machine call, whose <c>sub</c> is a client id rather than a user id.
    /// Preferred over <see cref="User"/> when set, so the audit trail records the principal
    /// that actually made the change instead of an all-zero GUID.
    /// </summary>
    public string? UserRaw { get; set; }

    public string? Entity { get; set; }
    public AuditActionType AuditActionType { get; set; }
    public string? Ip { get; set; }
    public string? Tenant { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
    public LogAudit ToLogAudit()
    {
        var log = new LogAudit();
        log.Timestamp = DateTime.Now;
        log.User = UserRaw ?? User.ToString();
        log.Ip = Ip;
        log.Tenant = Tenant;
        log.Entity = Entity;
        log.Action = AuditActionType.ToString();
        log.KeyValue = JsonConvert.SerializeObject(KeyValues);
        log.OldValue = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
        log.NewValue = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
        return log;
    }

}