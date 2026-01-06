using Payments.Domain.Enums;

namespace Payments.Application.Interfaces;

public interface IPaymentProviderFactory
{
    IPaymentProviderService GetProvider(PaymentProvider provider);
}
