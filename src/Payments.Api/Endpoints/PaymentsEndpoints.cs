using Payments.Api.Filters;
using Payments.Application.DTOs;
using Payments.Application.UseCases;

namespace Payments.Api.Endpoints;

public static class PaymentsEndpoints
{
    public static void MapPaymentsEndpoints(this WebApplication app)
    {
        app.MapPost("/payments", CreatePaymentAsync)
            .WithName("CreatePayment")
            .WithTags("Payments")
            .WithValidation<CreatePaymentRequest>()
            .Produces<CreatePaymentResponse>(StatusCodes.Status201Created)
            .Produces<CreatePaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CreatePaymentUseCase useCase,
        CancellationToken cancellationToken)
    {
        (CreatePaymentResponse response, bool created) = await useCase.ExecuteAsync(request, cancellationToken);

        return created
            ? Results.Created($"/payments/{response.Id}", response)
            : Results.Ok(response);
    }
}
