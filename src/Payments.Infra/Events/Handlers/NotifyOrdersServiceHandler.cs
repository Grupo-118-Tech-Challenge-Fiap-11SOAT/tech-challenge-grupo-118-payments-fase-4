using Microsoft.Extensions.Logging;
using Payments.Application.Events;
using Payments.Application.Interfaces;

namespace Payments.Infra.Events.Handlers;

public sealed class NotifyOrdersServiceHandler : IEventHandler<PaymentApprovedEvent>
{
    private readonly IOrdersApiClient _ordersApiClient;
    private readonly ILogger<NotifyOrdersServiceHandler> _logger;

    public NotifyOrdersServiceHandler(
        IOrdersApiClient ordersApiClient,
        ILogger<NotifyOrdersServiceHandler> logger)
    {
        _ordersApiClient = ordersApiClient;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentApprovedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Notifying Orders service about payment approval. OrderId: {OrderId}, PaymentId: {PaymentId}",
            @event.OrderId,
            @event.PaymentId);

        try
        {
            await _ordersApiClient.NotifyPaymentApprovedAsync(
                @event.OrderId,
                @event.PaymentId,
                cancellationToken);

            _logger.LogInformation(
                "Orders service notified successfully. OrderId: {OrderId}",
                @event.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to notify Orders service. OrderId: {OrderId}, PaymentId: {PaymentId}",
                @event.OrderId,
                @event.PaymentId);
        }
    }
}
