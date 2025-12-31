namespace Payments.Infra.Configuration;

public sealed class OrdersApiOptions
{
    public const string SectionName = "OrdersApi";

    public string BaseUrl { get; set; } = string.Empty;
    public string PaymentConfirmationEndpoint { get; set; } = "/orders/{orderId}/payment-confirmed";
}
