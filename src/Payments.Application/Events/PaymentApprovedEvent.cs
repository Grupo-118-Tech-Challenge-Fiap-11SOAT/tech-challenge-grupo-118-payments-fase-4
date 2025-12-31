namespace Payments.Application.Events;

public sealed class PaymentApprovedEvent : IEvent
{
    public long PaymentId { get; }
    public string OrderId { get; }
    public decimal Amount { get; }
    public DateTime OccurredAt { get; }

    public PaymentApprovedEvent(long paymentId, string orderId, decimal amount)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        OccurredAt = DateTime.UtcNow;
    }
}
