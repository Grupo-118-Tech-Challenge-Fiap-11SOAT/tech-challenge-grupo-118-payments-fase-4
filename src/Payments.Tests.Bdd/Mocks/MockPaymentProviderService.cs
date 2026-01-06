using Payments.Application.DTOs;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Tests.Bdd.Mocks;

public class MockPaymentProviderService : IPaymentProviderService
{
    public Task<PaymentProviderResult> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        // Simula resposta do MercadoPago sem fazer requisição real
        var qrCodeData = $"00020126580014br.gov.bcb.pix0136{payment.Uuid}520400005303986540{payment.Value.Amount:F2}5802BR5925TOME_LANCHES6009SAO_PAULO62070503***6304";
        
        return Task.FromResult(PaymentProviderResult.Ok(qrCodeData));
    }
}

