using Payments.Application.DTOs;
using Payments.Domain.Entities;

namespace Payments.Application.Interfaces;

public interface IPaymentProviderService
{
    Task<PaymentProviderResult> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}
