using Payments.Domain.Entities;
using Payments.Domain.Enums;

namespace Payments.Tests.Domain;

public class PaymentTests
{
    [Fact]
    public void When_CreatingPayment_WithValidData_Expect_PendingStatus()
    {
        // Arrange
        string orderId = "order-123";
        decimal value = 100m;
        PaymentProvider provider = PaymentProvider.MercadoPago;

        // Act
        Payment payment = Payment.Create(orderId, value, provider);

        // Assert
        Assert.Equal(orderId, payment.OrderId);
        Assert.Equal(100m, payment.Value.Amount);
        Assert.Equal(provider, payment.Provider);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.NotEqual(Guid.Empty, payment.Uuid);
    }

    [Fact]
    public void When_CreatingPayment_WithValidData_Expect_UuidGenerated()
    {
        // Arrange & Act
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Assert
        Assert.NotEqual(Guid.Empty, payment.Uuid);
    }

    [Fact]
    public void When_CreatingPayment_WithValidData_Expect_CreatedAtSet()
    {
        // Arrange
        DateTime before = DateTime.UtcNow;

        // Act
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Assert
        Assert.True(payment.CreatedAt >= before);
        Assert.True(payment.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void When_CreatingPayment_WithEmptyOrderId_Expect_ArgumentException()
    {
        // Arrange
        string orderId = "";
        decimal value = 100m;
        PaymentProvider provider = PaymentProvider.MercadoPago;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Payment.Create(orderId, value, provider));
    }

    [Fact]
    public void When_CreatingPayment_WithWhitespaceOrderId_Expect_ArgumentException()
    {
        // Arrange
        string orderId = "   ";
        decimal value = 100m;
        PaymentProvider provider = PaymentProvider.MercadoPago;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Payment.Create(orderId, value, provider));
    }

    [Fact]
    public void When_CreatingPayment_WithNegativeValue_Expect_ArgumentException()
    {
        // Arrange
        string orderId = "order-123";
        decimal value = -100m;
        PaymentProvider provider = PaymentProvider.MercadoPago;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Payment.Create(orderId, value, provider));
    }

    [Fact]
    public void When_SettingUserPaymentCode_WithValidCode_Expect_Success()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        string userPaymentCode = "QR_CODE_DATA_HERE";

        // Act
        payment.SetUserPaymentCode(userPaymentCode);

        // Assert
        Assert.Equal(userPaymentCode, payment.UserPaymentCode);
        Assert.NotNull(payment.UpdatedAt);
    }

    [Fact]
    public void When_SettingUserPaymentCode_WithEmptyCode_Expect_ArgumentException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.SetUserPaymentCode(""));
    }

    [Fact]
    public void When_SettingUserPaymentCode_WithWhitespaceCode_Expect_ArgumentException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.SetUserPaymentCode("   "));
    }

    [Fact]
    public void When_ApprovingPendingPayment_Expect_ApprovedStatus()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Act
        payment.Approve();

        // Assert
        Assert.Equal(PaymentStatus.Approved, payment.Status);
        Assert.NotNull(payment.UpdatedAt);
    }

    [Fact]
    public void When_ApprovingApprovedPayment_Expect_InvalidOperationException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Approve();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => payment.Approve());
    }

    [Fact]
    public void When_RejectingPendingPayment_Expect_RejectedStatus()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Act
        payment.Reject();

        // Assert
        Assert.Equal(PaymentStatus.Rejected, payment.Status);
        Assert.NotNull(payment.UpdatedAt);
    }

    [Fact]
    public void When_RejectingApprovedPayment_Expect_InvalidOperationException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Approve();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => payment.Reject());
    }

    [Fact]
    public void When_CancellingPendingPayment_Expect_CancelledStatus()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);

        // Act
        payment.Cancel();

        // Assert
        Assert.Equal(PaymentStatus.Cancelled, payment.Status);
        Assert.NotNull(payment.UpdatedAt);
    }

    [Fact]
    public void When_CancellingApprovedPayment_Expect_InvalidOperationException()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Approve();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => payment.Cancel());
    }
}
