using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Payments.Application.Interfaces;
using Payments.Infra.Configuration;

namespace Payments.Infra.ExternalServices.Orders;

public sealed class OrdersApiClient : IOrdersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OrdersApiOptions _options;

    public OrdersApiClient(HttpClient httpClient, IOptions<OrdersApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task NotifyPaymentApprovedAsync(
        string orderId,
        long paymentId,
        CancellationToken cancellationToken = default)
    {
        string endpoint = _options.PaymentConfirmationEndpoint.Replace("{orderId}", orderId);

        var payload = new
        {
            paymentId,
            status = "approved",
            confirmedAt = DateTime.UtcNow
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
