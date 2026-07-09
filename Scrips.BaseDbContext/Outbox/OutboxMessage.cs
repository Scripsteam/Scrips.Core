using System.Text.Json;
using Scrips.Core.Models.Audit;

namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Transactional-outbox row. Written into the SAME <c>SaveChanges</c> as the business data
/// (see <see cref="AuditableBaseDbContext"/>), so the audit event is atomic with the change
/// it describes. A background drainer publishes rows to Dapr off the request path.
/// One row per <c>SaveChanges</c> — Payload is the serialized change-list, matching the
/// legacy inline publish wire format (<c>List&lt;LogAudit&gt;</c>).
/// </summary>
public class OutboxMessage
{
    /// <summary>Identity — drain order.</summary>
    public long Id { get; set; }

    /// <summary>Stable idempotency key, assigned at insert. Published as the CloudEvent id
    /// (<c>cloudevent.id</c>) so the consumer can dedupe on it (insert-if-not-exists on LogAudit).</summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Best-effort tenant scope, from the ambient tenant context at write time. Null for the
    /// non-multitenant base context and for system/background writes with no tenant. Downstream
    /// keys/dedupes on <see cref="EventId"/> (not TenantId), and the serialized change-list in
    /// <see cref="Payload"/> carries the per-entity tenant — so a null here does not drop tenant info.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>Dapr topic (default "SaveAudit"; extensible to other events).</summary>
    public string Topic { get; set; } = "SaveAudit";

    /// <summary>Serialized change-list (PHI — see retention/at-rest encryption).</summary>
    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; }

    /// <summary><see cref="OutboxStatus"/>: 0 Pending, 1 Claimed, 2 Published, 3 DeadLetter.</summary>
    public byte Status { get; set; }

    /// <summary>Publish failures only — drives dead-lettering. NOT incremented on claim/reclaim.</summary>
    public int Attempts { get; set; }

    /// <summary>Claim/reclaim count — churn signal, kept off the dead-letter budget.</summary>
    public int ClaimCount { get; set; }

    /// <summary>Recovery CYCLES attempted by the scheduled DeadLetter recovery pass (PROD-1481). Distinct from
    /// <see cref="Attempts"/> (per-cycle publish failures) — caps how many times a dead-lettered row is re-queued
    /// before it's treated as terminal-poison. Original <see cref="Attempts"/> is reset to 0 on each requeue so
    /// a fresh publish budget applies; this counter is what bounds the retries.</summary>
    public int RecoveryAttempts { get; set; }

    /// <summary>Set on claim; drives the reaper visibility-timeout.</summary>
    public DateTime? ClaimedUtc { get; set; }

    /// <summary>Set on publish; drives retention purge.</summary>
    public DateTime? ProcessedUtc { get; set; }

    /// <summary>Set when a terminal-poison row's clinical <c>Payload</c> is scrubbed to metadata-only
    /// (PROD-1481 scrub-never-delete). Non-null ⇒ no clinical content remains; row is kept for the audit trail.</summary>
    public DateTime? ScrubbedUtc { get; set; }

    /// <summary>Payload after scrub — an empty change-list. Stays valid <c>List&lt;LogAudit&gt;</c> JSON,
    /// carries no clinical content.</summary>
    public const string ScrubbedPayload = "[]";

    public static OutboxMessage ForAudit(List<LogAudit> changes, string? tenantId) => new()
    {
        EventId = Guid.NewGuid(),
        TenantId = tenantId,
        Topic = "SaveAudit",
        Payload = JsonSerializer.Serialize(changes),
        CreatedUtc = DateTime.UtcNow,
        Status = OutboxStatus.Pending,
    };
}

public static class OutboxStatus
{
    public const byte Pending = 0;
    public const byte Claimed = 1;
    public const byte Published = 2;
    public const byte DeadLetter = 3;
}
