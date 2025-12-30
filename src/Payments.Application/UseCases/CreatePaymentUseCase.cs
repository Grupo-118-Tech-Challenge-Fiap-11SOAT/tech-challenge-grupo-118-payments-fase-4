using Payments.Application.DTOs;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;
using Payments.Domain.Enums;

namespace Payments.Application.UseCases;

public sealed class CreatePaymentUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentProviderFactory _providerFactory;

    public CreatePaymentUseCase(
        IPaymentRepository paymentRepository,
        IPaymentProviderFactory providerFactory)
    {
        _paymentRepository = paymentRepository;
        _providerFactory = providerFactory;
    }

    public async Task<(CreatePaymentResponse Response, bool Created)> ExecuteAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        Payment? existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

        if (existingPayment is not null)
        {
            return (CreatePaymentResponse.FromEntity(existingPayment), Created: false);
        }

        Payment payment = Payment.Create(request.OrderId, request.Value, PaymentProvider.MercadoPago);

        IPaymentProviderService providerService = _providerFactory.GetProvider(payment.Provider);
        PaymentProviderResult result = await providerService.ProcessPaymentAsync(payment, cancellationToken);
        if (!result.Success)
        {
            throw new InvalidOperationException($"Payment processing failed: {result.ErrorMessage}");
        }

        payment.SetUserPaymentCode(result.UserPaymentCode!);
        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return (CreatePaymentResponse.FromEntity(payment), Created: true);
    }
}
