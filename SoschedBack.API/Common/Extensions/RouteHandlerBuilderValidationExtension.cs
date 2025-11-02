using SoschedBack.Common.Filters;

namespace SoschedBack.Common.Extensions;

public static class RouteHandlerBuilderValidationExtension
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}