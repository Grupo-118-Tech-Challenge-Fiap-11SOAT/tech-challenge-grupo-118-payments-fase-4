using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payments.Application.Events;
using Payments.Application.Interfaces;
using Payments.Infra.Events;

namespace Payments.Tests.Infra;

public class InMemoryEventDispatcherTests
{
    private readonly ILogger<InMemoryEventDispatcher> _logger;

    public InMemoryEventDispatcherTests()
    {
        _logger = Substitute.For<ILogger<InMemoryEventDispatcher>>();
    }

    [Fact]
    public async Task When_DispatchingEvent_WithRegisteredHandler_Expect_HandlerCalled()
    {
        // Arrange
        IEventHandler<PaymentApprovedEvent> handler = Substitute.For<IEventHandler<PaymentApprovedEvent>>();

        ServiceCollection services = new();
        services.AddSingleton(handler);
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        InMemoryEventDispatcher dispatcher = new(serviceProvider, _logger);
        PaymentApprovedEvent @event = new(1, "order-123", 100m);

        // Act
        await dispatcher.DispatchAsync(@event);

        // Assert
        await handler.Received(1).HandleAsync(@event, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_DispatchingEvent_WithMultipleHandlers_Expect_AllHandlersCalled()
    {
        // Arrange
        IEventHandler<PaymentApprovedEvent> handler1 = Substitute.For<IEventHandler<PaymentApprovedEvent>>();
        IEventHandler<PaymentApprovedEvent> handler2 = Substitute.For<IEventHandler<PaymentApprovedEvent>>();

        ServiceCollection services = new();
        services.AddSingleton(handler1);
        services.AddSingleton(handler2);
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        InMemoryEventDispatcher dispatcher = new(serviceProvider, _logger);
        PaymentApprovedEvent @event = new(1, "order-123", 100m);

        // Act
        await dispatcher.DispatchAsync(@event);

        // Assert
        await handler1.Received(1).HandleAsync(@event, Arg.Any<CancellationToken>());
        await handler2.Received(1).HandleAsync(@event, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_DispatchingEvent_WithNoHandlers_Expect_NoException()
    {
        // Arrange
        ServiceCollection services = new();
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        InMemoryEventDispatcher dispatcher = new(serviceProvider, _logger);
        PaymentApprovedEvent @event = new(1, "order-123", 100m);

        // Act & Assert (should not throw)
        await dispatcher.DispatchAsync(@event);
    }

    [Fact]
    public async Task When_HandlerThrows_Expect_ExceptionPropagated()
    {
        // Arrange
        IEventHandler<PaymentApprovedEvent> handler = Substitute.For<IEventHandler<PaymentApprovedEvent>>();
        handler.HandleAsync(Arg.Any<PaymentApprovedEvent>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Handler failed"));

        ServiceCollection services = new();
        services.AddSingleton(handler);
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        InMemoryEventDispatcher dispatcher = new(serviceProvider, _logger);
        PaymentApprovedEvent @event = new(1, "order-123", 100m);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.DispatchAsync(@event));
    }
}
