using NSubstitute;
using Payments.Application.DTOs;
using Payments.Application.Events;
using Payments.Application.Interfaces;
using Payments.Application.UseCases;
using Payments.Domain.Entities;
using Payments.Domain.Enums;

namespace Payments.Tests.Application;

public class ConfirmPaymentUseCaseTests
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ConfirmPaymentUseCase _useCase;

    public ConfirmPaymentUseCaseTests()
    {
        _paymentRepository = Substitute.For<IPaymentRepository>();
        _eventDispatcher = Substitute.For<IEventDispatcher>();
        _useCase = new ConfirmPaymentUseCase(_paymentRepository, _eventDispatcher);
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithValidPendingPayment_Expect_ApprovedStatus()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        Guid paymentUuid = payment.Uuid;

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns(payment);

        // Act
        ConfirmPaymentResponse response = await _useCase.ExecuteAsync(paymentUuid);

        // Assert
        Assert.Equal("Approved", response.Status);
        Assert.Equal("order-123", response.OrderId);
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithValidPayment_Expect_RepositoryUpdateCalled()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        Guid paymentUuid = payment.Uuid;

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns(payment);

        // Act
        await _useCase.ExecuteAsync(paymentUuid);

        // Assert
        await _paymentRepository.Received(1).UpdateAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
        await _paymentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithValidPayment_Expect_EventDispatched()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        Guid paymentUuid = payment.Uuid;

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns(payment);

        // Act
        await _useCase.ExecuteAsync(paymentUuid);

        // Assert
        await _eventDispatcher.Received(1).DispatchAsync(
            Arg.Is<PaymentApprovedEvent>(e =>
                e.OrderId == "order-123" &&
                e.Amount == 100m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithNonExistentPayment_Expect_KeyNotFoundException()
    {
        // Arrange
        Guid paymentUuid = Guid.NewGuid();

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        // Act & Assert
        KeyNotFoundException exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _useCase.ExecuteAsync(paymentUuid));

        Assert.Contains(paymentUuid.ToString(), exception.Message);
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithAlreadyApprovedPayment_Expect_InvalidOperationException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Approve();
        Guid paymentUuid = payment.Uuid;

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns(payment);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(paymentUuid));
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithRejectedPayment_Expect_InvalidOperationException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Reject();
        Guid paymentUuid = payment.Uuid;

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns(payment);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(paymentUuid));
    }

    [Fact]
    public async Task When_ConfirmingPayment_WithCancelledPayment_Expect_InvalidOperationException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Cancel();
        Guid paymentUuid = payment.Uuid;

        _paymentRepository.GetByUuidAsync(paymentUuid, Arg.Any<CancellationToken>())
            .Returns(payment);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(paymentUuid));
    }
}
