namespace Payments.Infra.Configuration;

public sealed class MercadoPagoOptions
{
    public const string SectionName = "MercadoPago";

    public string BaseUrl { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string PosId { get; set; } = string.Empty;
    public string NotificationUrl { get; set; } = string.Empty;
}
