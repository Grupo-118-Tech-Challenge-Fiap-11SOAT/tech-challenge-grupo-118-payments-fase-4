using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payments.Application.Events;
using Payments.Application.Interfaces;

namespace Payments.Infra.Events;

public sealed class InMemoryEventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventDispatcher> _logger;

    public InMemoryEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<InMemoryEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        Type handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        IEnumerable<object?> handlers = _serviceProvider.GetServices(handlerType);

        foreach (object? handler in handlers)
        {
            if (handler is null) continue;

            try
            {
                dynamic dynamicHandler = handler;
                await dynamicHandler.HandleAsync((dynamic)@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling event {EventType}", typeof(TEvent).Name);
                throw;
            }
        }
    }
}
