using Payments.Domain.Entities;

namespace Payments.Application.DTOs;

public sealed class ConfirmPaymentResponse
{
    public long PaymentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public static ConfirmPaymentResponse FromEntity(Payment payment)
    {
        return new ConfirmPaymentResponse
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Status = payment.Status.ToString()
        };
    }
}
