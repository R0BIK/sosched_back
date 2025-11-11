using Microsoft.AspNetCore.Http.HttpResults;
using SoschedBack.Common;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Common.UnifiedResponse;

namespace SoschedBack.Auth.Endpoints.CheckAuth;

public class CheckAuth : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapGet("/me", Handle)
            .WithSummary("Returns data about the currently authenticated user");
    }

    private static async Task<Ok<Result<Response>>> Handle(
        ITokenHandlerService tokenHandlerService,
        CancellationToken cancellationToken
    )
    {
        var userResult = await tokenHandlerService.GetCurrentUser();

        if (userResult.IsFailure || userResult.Data == null)
        {
            var error = Error.From("Unauthorized", "UNAUTHORIZED");
            return TypedResults.Ok(Result.Failure<Response>(error));
        }

        var user = userResult.Data;

        var response = new Response(
            user.Id
        );

        return TypedResults.Ok(Result.Success(response));
    }

    private sealed record Response(
        int Id
    );
}