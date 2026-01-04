using Payments.Application.Events;

namespace Payments.Application.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
