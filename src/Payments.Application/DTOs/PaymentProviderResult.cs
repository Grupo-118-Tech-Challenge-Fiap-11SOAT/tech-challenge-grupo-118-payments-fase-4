namespace Payments.Application.DTOs;

public sealed class PaymentProviderResult
{
    public bool Success { get; private set; }
    public string? UserPaymentCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    private PaymentProviderResult() { }

    public static PaymentProviderResult Ok(string userPaymentCode)
    {
        return new PaymentProviderResult
        {
            Success = true,
            UserPaymentCode = userPaymentCode
        };
    }

    public static PaymentProviderResult Fail(string errorMessage)
    {
        return new PaymentProviderResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
