using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scrips.BaseDbContext.Outbox;

public static class AuditOutboxServiceCollectionExtensions
{
    /// <summary>
    /// Wires the audit outbox for a service. Binds <see cref="AuditOutboxOptions"/> from the
    /// "Audit" config section, sets the process-wide writer flag from <c>Audit:UseOutbox</c>
    /// (default off), and — only when enabled — registers the drainer for the service's context.
    /// Call once at startup: <c>services.AddAuditOutbox&lt;SchedulingDbContext&gt;(Configuration)</c>.
    /// With the flag off this is a no-op beyond binding options — zero behavior change.
    /// </summary>
    public static IServiceCollection AddAuditOutbox<TContext>(this IServiceCollection services,
        IConfiguration configuration) where TContext : DbContext
    {
        services.Configure<AuditOutboxOptions>(configuration.GetSection("Audit"));

        var enabled = configuration.GetValue<bool>("Audit:UseOutbox");
        AuditOutboxRuntime.Enabled = enabled;

        if (enabled)
            services.AddHostedService<OutboxDrainerService<TContext>>();

        return services;
    }
}
