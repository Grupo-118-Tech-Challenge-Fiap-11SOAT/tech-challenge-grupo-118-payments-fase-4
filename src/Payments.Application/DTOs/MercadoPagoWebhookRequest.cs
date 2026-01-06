using System.Text.Json.Serialization;

namespace Payments.Application.DTOs;

public sealed class MercadoPagoWebhookRequest
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("api_version")]
    public string ApiVersion { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public MercadoPagoWebhookData Data { get; set; } = new();

    [JsonPropertyName("date_created")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("live_mode")]
    public bool LiveMode { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
}

public sealed class MercadoPagoWebhookData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
