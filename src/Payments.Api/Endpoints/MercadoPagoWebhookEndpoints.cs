using System.Diagnostics.CodeAnalysis;
using Payments.Application.DTOs;
using Payments.Application.UseCases;

namespace Payments.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class MercadoPagoWebhookEndpoints
{
    private const string PaymentCreatedAction = "payment.created";

    public static void MapMercadoPagoWebhookEndpoints(this WebApplication app)
    {
        app.MapPost("/payments/webhooks/mercado-pago/{paymentUuid:guid}", HandleWebhookAsync)
            .WithName("MercadoPagoWebhook")
            .WithTags("Webhooks")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> HandleWebhookAsync(
        Guid paymentUuid,
        MercadoPagoWebhookRequest request,
        ConfirmPaymentUseCase useCase,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(request.Action, PaymentCreatedAction, StringComparison.OrdinalIgnoreCase))
        {
            return Results.Ok();
        }

        ConfirmPaymentResponse response = await useCase.ExecuteAsync(paymentUuid, cancellationToken);

        return Results.Ok(response);
    }
}
