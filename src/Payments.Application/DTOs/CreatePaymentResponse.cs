using Payments.Domain.Entities;

namespace Payments.Application.DTOs;

public sealed class CreatePaymentResponse
{
    public long Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string PaymentProvider { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string Status { get; set; } = string.Empty;
    public string UserPaymentCode { get; set; } = string.Empty;

    public static CreatePaymentResponse FromEntity(Payment payment)
    {
        return new CreatePaymentResponse
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Value = payment.Value.Amount,
            PaymentProvider = payment.Provider.ToString(),
            Uuid = payment.Uuid,
            Status = payment.Status.ToString(),
            UserPaymentCode = payment.UserPaymentCode
        };
    }
}
