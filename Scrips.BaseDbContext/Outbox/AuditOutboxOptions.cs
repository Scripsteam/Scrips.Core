namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Bound from the "Audit" config section. The writer is OFF by default — flipping
/// <see cref="UseOutbox"/> per service is the rollout lever and the instant rollback.
/// </summary>
public class AuditOutboxOptions
{
    /// <summary>Config key: <c>Audit:UseOutbox</c>. When false, the legacy inline Dapr publish is used.</summary>
    public bool UseOutbox { get; set; } = false;

    public string PubSubName { get; set; } = "pubsub";

    /// <summary>Rows claimed per drain iteration.</summary>
    public int BatchSize { get; set; } = 200;

    /// <summary>Publish failures before a row is dead-lettered.</summary>
    public int MaxAttempts { get; set; } = 5;

    /// <summary>Drainer poll interval (seconds).</summary>
    public int PollSeconds { get; set; } = 5;

    /// <summary>Per-publish timeout — bounds a hung Dapr sidecar so one stuck publish can't stall the drain loop.
    /// Keep it well below <see cref="VisibilityTimeoutMinutes"/> so a timed-out publish is retried before the reaper reclaims the row.</summary>
    public int PublishTimeoutSeconds { get; set; } = 30;

    /// <summary>Reaper visibility timeout — MUST exceed the max time a healthy batch takes to publish.</summary>
    public int VisibilityTimeoutMinutes { get; set; } = 5;

    /// <summary>Retention: Published rows are purged after this many days — bounds the table and the PHI-at-rest
    /// window (PROD-1479). DeadLetter rows are NOT auto-purged (retained for investigation). 0 or less disables purge.</summary>
    public int PublishedRetentionDays { get; set; } = 7;

    /// <summary>Rows deleted per purge pass — one bounded DELETE per drain tick, so retention never runs long.</summary>
    public int PurgeBatchSize { get; set; } = 1000;

    // ---- DeadLetter recovery + retention (PROD-1481) ----

    /// <summary>Scheduled DeadLetter recovery pass: re-queue dead-lettered rows so the normal drainer
    /// re-publishes them (transient outages self-heal into LogAudit). Re-publish only — never a direct
    /// audit-DB write. Off → dead-letters stay put for manual investigation.</summary>
    public bool RecoveryEnabled { get; set; } = true;

    /// <summary>UTC hour (0–23) at which the daily recovery + scrub pass runs — keep it off-peak.
    /// The pass fires once per day the process is alive during this hour.</summary>
    public int RecoveryRunAtUtcHour { get; set; } = 2;

    /// <summary>Recovery CYCLES before a row is treated as terminal-poison. Distinct from <see cref="MaxAttempts"/>
    /// (per-cycle publish failures): each recovery cycle resets Attempts to give a fresh publish budget, so this
    /// is "how many nightly full-retry cycles" — sized for multi-day outage tolerance.</summary>
    public int RecoveryMaxAttempts { get; set; } = 5;

    /// <summary>Rows re-queued / scrubbed per recovery pass — bounded like the drain/purge batches.</summary>
    public int RecoveryBatchSize { get; set; } = 200;

    /// <summary>Retention (PROD-1481 / UAE Federal Law 2/2019 — ≥25yr health-data retention + no premature
    /// destruction): terminal-poison DeadLetter rows are NEVER deleted. When true, once a row exhausts
    /// <see cref="RecoveryMaxAttempts"/> its clinical <c>Payload</c> is scrubbed to an empty change-list
    /// (metadata row kept forever) — bounds PHI-at-rest without destroying the audit-trail record.
    /// Default OFF: enabling scrub-on-terminal is a compliance decision, gated on the PROD-1481 sign-off.</summary>
    public bool ScrubTerminalPayload { get; set; } = false;
}

/// <summary>
/// Process-wide flag read by the base DbContexts to choose outbox-write vs inline publish.
/// Set once at startup by <c>AddAuditOutbox</c>; read-only thereafter. Each service is its own
/// process, so the static reflects that service's config.
/// </summary>
internal static class AuditOutboxRuntime
{
    public static volatile bool Enabled;
}
