using Payments.Application.DTOs;
using Payments.Domain.Entities;
using Payments.Domain.Enums;

namespace Payments.Tests.Application;

public class DTOTests
{
    [Fact]
    public void When_CreatingPaymentProviderResultOk_Expect_SuccessTrue()
    {
        // Arrange & Act
        PaymentProviderResult result = PaymentProviderResult.Ok("QR_CODE_DATA");

        // Assert
        Assert.True(result.Success);
        Assert.Equal("QR_CODE_DATA", result.UserPaymentCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void When_CreatingPaymentProviderResultFail_Expect_SuccessFalse()
    {
        // Arrange & Act
        PaymentProviderResult result = PaymentProviderResult.Fail("Error occurred");

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.UserPaymentCode);
        Assert.Equal("Error occurred", result.ErrorMessage);
    }

    [Fact]
    public void When_CreatingCreatePaymentRequest_Expect_PropertiesSet()
    {
        // Arrange & Act
        CreatePaymentRequest request = new()
        {
            OrderId = "order-123",
            Value = 100.50m
        };

        // Assert
        Assert.Equal("order-123", request.OrderId);
        Assert.Equal(100.50m, request.Value);
    }

    [Fact]
    public void When_CreatingConfirmPaymentResponseFromEntity_Expect_PropertiesMapped()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.Approve();

        // Act
        ConfirmPaymentResponse response = ConfirmPaymentResponse.FromEntity(payment);

        // Assert
        Assert.Equal(payment.Id, response.PaymentId);
        Assert.Equal("order-123", response.OrderId);
        Assert.Equal("Approved", response.Status);
    }

    [Fact]
    public void When_CreatingCreatePaymentResponseFromEntity_Expect_PropertiesMapped()
    {
        // Arrange
        Payment payment = Payment.Create("order-123", 100m, PaymentProvider.MercadoPago);
        payment.SetUserPaymentCode("QR_CODE_DATA");

        // Act
        CreatePaymentResponse response = CreatePaymentResponse.FromEntity(payment);

        // Assert
        Assert.Equal(payment.Id, response.Id);
        Assert.Equal("order-123", response.OrderId);
        Assert.Equal(100m, response.Value);
        Assert.Equal("MercadoPago", response.PaymentProvider);
        Assert.Equal(payment.Uuid, response.Uuid);
        Assert.Equal("Pending", response.Status);
        Assert.Equal("QR_CODE_DATA", response.UserPaymentCode);
    }

    [Fact]
    public void When_CreatingMercadoPagoWebhookRequest_Expect_DefaultValuesSet()
    {
        // Arrange & Act
        MercadoPagoWebhookRequest request = new();

        // Assert
        Assert.Equal(string.Empty, request.Action);
        Assert.Equal(string.Empty, request.ApiVersion);
        Assert.NotNull(request.Data);
        Assert.Equal(default, request.DateCreated);
        Assert.Equal(0, request.Id);
        Assert.False(request.LiveMode);
        Assert.Equal(string.Empty, request.Type);
        Assert.Equal(string.Empty, request.UserId);
    }

    [Fact]
    public void When_SettingMercadoPagoWebhookRequestProperties_Expect_PropertiesSet()
    {
        // Arrange
        DateTime dateCreated = new(2024, 1, 1, 12, 0, 0);

        // Act
        MercadoPagoWebhookRequest request = new()
        {
            Action = "payment.created",
            ApiVersion = "v1",
            Data = new MercadoPagoWebhookData { Id = "12345" },
            DateCreated = dateCreated,
            Id = 67890,
            LiveMode = true,
            Type = "payment",
            UserId = "user-123"
        };

        // Assert
        Assert.Equal("payment.created", request.Action);
        Assert.Equal("v1", request.ApiVersion);
        Assert.Equal("12345", request.Data.Id);
        Assert.Equal(dateCreated, request.DateCreated);
        Assert.Equal(67890, request.Id);
        Assert.True(request.LiveMode);
        Assert.Equal("payment", request.Type);
        Assert.Equal("user-123", request.UserId);
    }

    [Fact]
    public void When_CreatingMercadoPagoWebhookData_Expect_DefaultValuesSet()
    {
        // Arrange & Act
        MercadoPagoWebhookData data = new();

        // Assert
        Assert.Equal(string.Empty, data.Id);
    }

    [Fact]
    public void When_SettingMercadoPagoWebhookDataId_Expect_IdSet()
    {
        // Arrange & Act
        MercadoPagoWebhookData data = new() { Id = "payment-id-123" };

        // Assert
        Assert.Equal("payment-id-123", data.Id);
    }
}
