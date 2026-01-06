using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Payments.Api.Filters;

[ExcludeFromCodeCoverage]
public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        T? argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is null)
        {
            return Results.BadRequest(new { error = "Request body is required" });
        }

        List<ValidationResult> validationResults = new();
        ValidationContext validationContext = new(argument);

        if (!Validator.TryValidateObject(argument, validationContext, validationResults, validateAllProperties: true))
        {
            var errors = validationResults
                .Where(r => r.ErrorMessage is not null)
                .Select(r => r.ErrorMessage!)
                .ToList();

            return Results.BadRequest(new { errors });
        }

        return await next(context);
    }
}
