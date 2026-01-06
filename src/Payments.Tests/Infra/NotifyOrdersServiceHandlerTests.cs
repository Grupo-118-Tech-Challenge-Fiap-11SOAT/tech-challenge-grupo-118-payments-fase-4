using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Payments.Application.Events;
using Payments.Application.Interfaces;
using Payments.Infra.Events.Handlers;

namespace Payments.Tests.Infra;

public class NotifyOrdersServiceHandlerTests
{
    private readonly IOrdersApiClient _ordersApiClient;
    private readonly ILogger<NotifyOrdersServiceHandler> _logger;
    private readonly NotifyOrdersServiceHandler _handler;

    public NotifyOrdersServiceHandlerTests()
    {
        _ordersApiClient = Substitute.For<IOrdersApiClient>();
        _logger = Substitute.For<ILogger<NotifyOrdersServiceHandler>>();
        _handler = new NotifyOrdersServiceHandler(_ordersApiClient, _logger);
    }

    [Fact]
    public async Task When_HandlingPaymentApprovedEvent_Expect_OrdersApiClientCalled()
    {
        // Arrange
        long paymentId = 12345;
        PaymentApprovedEvent @event = new(paymentId, "order-123", 100m);

        // Act
        await _handler.HandleAsync(@event);

        // Assert
        await _ordersApiClient.Received(1).NotifyPaymentApprovedAsync(
            "order-123",
            paymentId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_OrdersApiClientThrows_Expect_ExceptionPropagated()
    {
        // Arrange
        long paymentId = 12345;
        PaymentApprovedEvent @event = new(paymentId, "order-123", 100m);
        _ordersApiClient
            .NotifyPaymentApprovedAsync(Arg.Any<string>(), Arg.Any<long>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _handler.HandleAsync(@event));
    }
}
