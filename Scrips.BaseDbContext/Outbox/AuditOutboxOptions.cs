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
