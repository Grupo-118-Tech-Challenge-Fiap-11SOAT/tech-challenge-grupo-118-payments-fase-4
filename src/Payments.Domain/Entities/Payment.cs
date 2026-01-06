using Payments.Domain.Enums;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities;

public sealed class Payment
{
    public long Id { get; protected set; }
    public string OrderId { get; protected set; }
    public Money Value { get; protected set; }
    public PaymentProvider Provider { get; protected set; }
    public Guid Uuid { get; protected set; }
    public PaymentStatus Status { get; protected set; }
    public string? UserPaymentCode { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected Payment() { }

    private Payment(string orderId, Money value, PaymentProvider provider)
    {
        OrderId = orderId;
        Value = value;
        Provider = provider;
        Uuid = Guid.NewGuid();
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Payment Create(string orderId, decimal value, PaymentProvider provider)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        Money moneyValue = Money.Create(value);
        return new Payment(orderId, moneyValue, provider);
    }

    public void SetUserPaymentCode(string userPaymentCode)
    {
        if (string.IsNullOrWhiteSpace(userPaymentCode))
            throw new ArgumentException("User payment code cannot be empty", nameof(userPaymentCode));

        UserPaymentCode = userPaymentCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be approved");

        Status = PaymentStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be rejected");

        Status = PaymentStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be cancelled");

        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
