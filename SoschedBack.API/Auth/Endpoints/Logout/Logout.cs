using Microsoft.AspNetCore.Http.HttpResults;
using SoschedBack.Common;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Common.UnifiedResponse;

namespace SoschedBack.Auth.Endpoints.Logout;

public class Logout : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/logout", Handle);
    }
    
    private static async Task<Results<Ok<Result<Response>>, BadRequest<Result>>> Handle(
        ITokenHandlerService tokenHandlerService,
        CancellationToken cancellationToken)
    {
        await tokenHandlerService.Logout();

        var response = Result.Success(new Response("Successfully logged out."));
        return TypedResults.Ok(response);
    }
    
    private record Response(string Message);
}