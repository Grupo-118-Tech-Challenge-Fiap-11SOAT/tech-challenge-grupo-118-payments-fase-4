using Payments.Application.DTOs;

namespace Payments.Tests.Bdd.Context;

public class PaymentScenarioContext
{
    public CreatePaymentRequest? Request { get; set; }
    public CreatePaymentResponse? Response { get; set; }
    public Exception? Exception { get; set; }
    public bool WasCreated { get; set; }
}
