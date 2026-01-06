using System.Text.Json.Serialization;

namespace Payments.Infra.ExternalServices.MercadoPago;

public sealed class MercadoPagoQrRequest
{
    [JsonPropertyName("external_reference")]
    public string ExternalReference { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("notification_url")]
    public string NotificationUrl { get; set; } = string.Empty;

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("items")]
    public List<MercadoPagoItem> Items { get; set; } = new();
}

public sealed class MercadoPagoItem
{
    [JsonPropertyName("sku_number")]
    public string SkuNumber { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit_measure")]
    public string UnitMeasure { get; set; } = "unit";

    [JsonPropertyName("total_amount")]
    public decimal TotalAmount { get; set; }
}
