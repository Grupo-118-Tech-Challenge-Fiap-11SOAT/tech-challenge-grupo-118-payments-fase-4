using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Payments.Application.Interfaces;
using Payments.Infra.Configuration;

namespace Payments.Infra.ExternalServices.Orders;

[ExcludeFromCodeCoverage]
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

        HttpResponseMessage response = await _httpClient.PatchAsync(endpoint, null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
