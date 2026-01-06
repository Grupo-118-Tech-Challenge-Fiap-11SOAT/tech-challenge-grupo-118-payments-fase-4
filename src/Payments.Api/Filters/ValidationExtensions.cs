using System.Diagnostics.CodeAnalysis;

namespace Payments.Api.Filters;

[ExcludeFromCodeCoverage]
public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}
