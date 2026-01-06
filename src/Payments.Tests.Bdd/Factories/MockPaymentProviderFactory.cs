using Payments.Application.Interfaces;
using Payments.Domain.Enums;
using Payments.Tests.Bdd.Mocks;

namespace Payments.Tests.Bdd.Factories;

public class MockPaymentProviderFactory : IPaymentProviderFactory
{
    private readonly MockPaymentProviderService _mockService;

    public MockPaymentProviderFactory()
    {
        _mockService = new MockPaymentProviderService();
    }

    public IPaymentProviderService GetProvider(PaymentProvider provider)
    {
        // Retorna sempre o mock, independente do provider
        return _mockService;
    }
}

