using Payments.Application.DTOs;
using Payments.Application.Events;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.UseCases;

public sealed class ConfirmPaymentUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventDispatcher _eventDispatcher;

    public ConfirmPaymentUseCase(
        IPaymentRepository paymentRepository,
        IEventDispatcher eventDispatcher)
    {
        _paymentRepository = paymentRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<ConfirmPaymentResponse> ExecuteAsync(
        Guid paymentUuid,
        CancellationToken cancellationToken = default)
    {
        Payment? payment = await _paymentRepository.GetByUuidAsync(paymentUuid, cancellationToken);

        if (payment is null)
        {
            throw new KeyNotFoundException($"Payment with UUID {paymentUuid} not found");
        }

        payment.Approve();

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        PaymentApprovedEvent approvedEvent = new(
            payment.Id,
            payment.OrderId,
            payment.Value.Amount);

        await _eventDispatcher.DispatchAsync(approvedEvent, cancellationToken);

        return ConfirmPaymentResponse.FromEntity(payment);
    }
}
