using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Interfaces;
using Payments.Domain.Enums;
using Payments.Infra.ExternalServices.MercadoPago;

namespace Payments.Infra.Factories;

public sealed class PaymentProviderFactory : IPaymentProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentProviderService GetProvider(PaymentProvider provider)
    {
        return provider switch
        {
            PaymentProvider.MercadoPago => _serviceProvider.GetRequiredService<MercadoPagoPaymentProviderService>(),
            _ => throw new NotSupportedException($"Payment provider '{provider}' is not supported")
        };
    }
}
