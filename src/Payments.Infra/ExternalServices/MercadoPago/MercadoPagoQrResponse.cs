using System.Text.Json.Serialization;

namespace Payments.Infra.ExternalServices.MercadoPago;

public sealed class MercadoPagoQrResponse
{
    [JsonPropertyName("in_store_order_id")]
    public string InStoreOrderId { get; set; } = string.Empty;

    [JsonPropertyName("qr_data")]
    public string QrData { get; set; } = string.Empty;
}
