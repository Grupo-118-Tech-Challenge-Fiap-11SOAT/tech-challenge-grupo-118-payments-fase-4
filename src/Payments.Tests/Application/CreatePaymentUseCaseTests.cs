using NSubstitute;
using Payments.Application.DTOs;
using Payments.Application.Interfaces;
using Payments.Application.UseCases;
using Payments.Domain.Entities;
using Payments.Domain.Enums;

namespace Payments.Tests.Application;

public class CreatePaymentUseCaseTests
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentProviderFactory _providerFactory;
    private readonly IPaymentProviderService _providerService;
    private readonly CreatePaymentUseCase _useCase;

    public CreatePaymentUseCaseTests()
    {
        _paymentRepository = Substitute.For<IPaymentRepository>();
        _providerFactory = Substitute.For<IPaymentProviderFactory>();
        _providerService = Substitute.For<IPaymentProviderService>();
        _useCase = new CreatePaymentUseCase(_paymentRepository, _providerFactory);
    }

    [Fact]
    public async Task When_CreatingPayment_WithValidRequest_Expect_SuccessfulResponse()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        _providerFactory.GetProvider(PaymentProvider.MercadoPago)
            .Returns(_providerService);

        _providerService.ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>())
            .Returns(PaymentProviderResult.Ok("QR_CODE_DATA"));

        // Act
        (CreatePaymentResponse response, bool created) = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.True(created);
        Assert.Equal("order-123", response.OrderId);
        Assert.Equal(100m, response.Value);
        Assert.Equal("MercadoPago", response.PaymentProvider);
        Assert.Equal("Pending", response.Status);
        Assert.Equal("QR_CODE_DATA", response.UserPaymentCode);
    }

    [Fact]
    public async Task When_CreatingPayment_WithValidRequest_Expect_RepositoryAddCalled()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        _providerFactory.GetProvider(PaymentProvider.MercadoPago)
            .Returns(_providerService);

        _providerService.ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>())
            .Returns(PaymentProviderResult.Ok("QR_CODE_DATA"));

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_CreatingPayment_WithValidRequest_Expect_ProviderProcessPaymentCalled()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        _providerFactory.GetProvider(PaymentProvider.MercadoPago)
            .Returns(_providerService);

        _providerService.ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>())
            .Returns(PaymentProviderResult.Ok("QR_CODE_DATA"));

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        await _providerService.Received(1).ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_CreatingPayment_WithValidRequest_Expect_RepositoryUpdateCalled()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        _providerFactory.GetProvider(PaymentProvider.MercadoPago)
            .Returns(_providerService);

        _providerService.ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>())
            .Returns(PaymentProviderResult.Ok("QR_CODE_DATA"));

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        await _paymentRepository.Received(1).UpdateAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_PaymentProviderFails_Expect_InvalidOperationException()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        _providerFactory.GetProvider(PaymentProvider.MercadoPago)
            .Returns(_providerService);

        _providerService.ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>())
            .Returns(PaymentProviderResult.Fail("API Error"));

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(request));

        Assert.Contains("Payment processing failed", exception.Message);
        Assert.Contains("API Error", exception.Message);
    }

    [Fact]
    public async Task When_PaymentProviderFails_Expect_RepositoryUpdateNotCalled()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        _providerFactory.GetProvider(PaymentProvider.MercadoPago)
            .Returns(_providerService);

        _providerService.ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>())
            .Returns(PaymentProviderResult.Fail("API Error"));

        // Act
        try
        {
            await _useCase.ExecuteAsync(request);
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        // Assert
        await _paymentRepository.DidNotReceive().UpdateAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_CreatingPayment_WithEmptyOrderId_Expect_ArgumentException()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "",
            Value = 100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task When_CreatingPayment_WithNegativeValue_Expect_ArgumentException()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = -100m
        };

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns((Payment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task When_PaymentAlreadyExistsForOrderId_Expect_ExistingPaymentReturned()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        Payment existingPayment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        existingPayment.SetUserPaymentCode("EXISTING_QR_CODE");

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns(existingPayment);

        // Act
        (CreatePaymentResponse response, bool created) = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.False(created);
        Assert.Equal("order-123", response.OrderId);
        Assert.Equal("EXISTING_QR_CODE", response.UserPaymentCode);
    }

    [Fact]
    public async Task When_PaymentAlreadyExistsForOrderId_Expect_RepositoryAddNotCalled()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        Payment existingPayment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        existingPayment.SetUserPaymentCode("EXISTING_QR_CODE");

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns(existingPayment);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        await _paymentRepository.DidNotReceive().AddAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task When_PaymentAlreadyExistsForOrderId_Expect_ProviderNotCalled()
    {
        // Arrange
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100m
        };

        Payment existingPayment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        existingPayment.SetUserPaymentCode("EXISTING_QR_CODE");

        _paymentRepository.GetByOrderIdAsync(request.OrderId, Arg.Any<CancellationToken>())
            .Returns(existingPayment);

        // Act
        await _useCase.ExecuteAsync(request);

        // Assert
        _providerFactory.DidNotReceive().GetProvider(Arg.Any<PaymentProvider>());
        await _providerService.DidNotReceive().ProcessPaymentAsync(Arg.Any<Payment>(), Arg.Any<CancellationToken>());
    }
}
