using Scrips.Core.Application.Common.Interfaces;
using Scrips.Core.Shared.Events;

namespace Scrips.Core.Application.Common.Events;

public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}