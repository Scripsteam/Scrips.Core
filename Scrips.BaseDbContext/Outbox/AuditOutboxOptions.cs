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

    /// <summary>Reaper visibility timeout — MUST exceed the max time a healthy batch takes to publish.</summary>
    public int VisibilityTimeoutMinutes { get; set; } = 5;
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
