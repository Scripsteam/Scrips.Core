using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Scrips.BaseDbContext.Outbox;

/// <summary>
/// Fail-fast guard: when the outbox writer is enabled, refuse to start if the service's
/// <typeparamref name="TContext"/> hasn't mapped <see cref="OutboxMessage"/> (i.e. it overrides
/// OnModelCreating without calling <c>base.OnModelCreating</c>). Without this, the flag flips,
/// the table never maps, and audit events are silently dropped with no error — unacceptable for
/// an audit trail. Registered before the drainer so it throws during startup.
/// </summary>
public class OutboxStartupValidator<TContext> : IHostedService where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxStartupValidator(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<TContext>();

        if (ctx.Model.FindEntityType(typeof(OutboxMessage)) is null)
        {
            throw new InvalidOperationException(
                $"Audit:UseOutbox is ON but {nameof(OutboxMessage)} is not mapped in {typeof(TContext).Name}. " +
                $"The service DbContext must call base.OnModelCreating(modelBuilder). Refusing to start — " +
                $"otherwise audit events would be silently dropped.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
