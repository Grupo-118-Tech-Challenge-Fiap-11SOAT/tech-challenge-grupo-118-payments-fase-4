using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Payments.Application.DTOs;
using Payments.Application.Interfaces;
using Payments.Domain.Entities;
using Payments.Infra.Configuration;

namespace Payments.Infra.ExternalServices.MercadoPago;

public sealed class MercadoPagoPaymentProviderService : IPaymentProviderService
{
    private readonly HttpClient _httpClient;
    private readonly MercadoPagoOptions _options;

    public MercadoPagoPaymentProviderService(
        HttpClient httpClient,
        IOptions<MercadoPagoOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<PaymentProviderResult> ProcessPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        string endpoint = $"/instore/orders/qr/seller/collectors/{_options.UserId}/pos/{_options.PosId}/qrs";

        MercadoPagoQrRequest request = new()
        {
            ExternalReference = $"order_{payment.OrderId}",
            Title = "Pedido de lanche",
            Description = "Pedido de lanche efetuado na loja TomeLanches",
            NotificationUrl = $"{_options.NotificationUrl}/{payment.Uuid}",
            TotalAmount = payment.Value.Amount,
            Items = new List<MercadoPagoItem>
            {
                new()
                {
                    SkuNumber = payment.OrderId,
                    Category = "food",
                    Title = "Pedido de lanche",
                    Description = "Pedido de lanche efetuado na loja TomeLanches",
                    UnitPrice = payment.Value.Amount,
                    Quantity = 1,
                    UnitMeasure = "unit",
                    TotalAmount = payment.Value.Amount
                }
            }
        };

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                endpoint,
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return PaymentProviderResult.Fail($"MercadoPago API error: {response.StatusCode} - {errorContent}");
            }

            MercadoPagoQrResponse? qrResponse = await response.Content.ReadFromJsonAsync<MercadoPagoQrResponse>(
                cancellationToken: cancellationToken);

            if (qrResponse is null || string.IsNullOrEmpty(qrResponse.QrData))
            {
                return PaymentProviderResult.Fail("Invalid response from MercadoPago API");
            }

            return PaymentProviderResult.Ok(qrResponse.QrData);
        }
        catch (HttpRequestException ex)
        {
            return PaymentProviderResult.Fail($"Network error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return PaymentProviderResult.Fail($"JSON parsing error: {ex.Message}");
        }
    }
}
