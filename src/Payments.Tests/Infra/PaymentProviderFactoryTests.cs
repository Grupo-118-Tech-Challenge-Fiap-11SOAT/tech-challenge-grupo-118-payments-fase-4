using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Payments.Application.Interfaces;
using Payments.Domain.Enums;
using Payments.Infra.Configuration;
using Payments.Infra.ExternalServices.MercadoPago;
using Payments.Infra.Factories;

namespace Payments.Tests.Infra;

public class PaymentProviderFactoryTests
{
    [Fact]
    public void When_GettingMercadoPagoProvider_Expect_MercadoPagoServiceReturned()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddHttpClient<MercadoPagoPaymentProviderService>();
        services.Configure<MercadoPagoOptions>(options =>
        {
            options.BaseUrl = "https://api.mercadopago.com";
            options.AccessToken = "test_token";
            options.UserId = "12345";
            options.PosId = "pos_001";
            options.NotificationUrl = "https://test.com/webhook";
        });

        ServiceProvider serviceProvider = services.BuildServiceProvider();
        PaymentProviderFactory factory = new(serviceProvider);

        // Act
        IPaymentProviderService result = factory.GetProvider(PaymentProvider.MercadoPago);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<MercadoPagoPaymentProviderService>(result);
    }

    [Fact]
    public void When_GettingUnsupportedProvider_Expect_NotSupportedException()
    {
        // Arrange
        ServiceCollection services = new();
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        PaymentProviderFactory factory = new(serviceProvider);

        // Act & Assert
        NotSupportedException exception = Assert.Throws<NotSupportedException>(
            () => factory.GetProvider((PaymentProvider)999));

        Assert.Contains("not supported", exception.Message);
    }
}
