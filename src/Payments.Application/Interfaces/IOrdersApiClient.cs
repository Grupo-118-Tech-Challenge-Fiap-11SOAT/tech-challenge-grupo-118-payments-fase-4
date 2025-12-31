namespace Payments.Application.Interfaces;

public interface IOrdersApiClient
{
    Task NotifyPaymentApprovedAsync(string orderId, long paymentId, CancellationToken cancellationToken = default);
}
