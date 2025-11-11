using Microsoft.AspNetCore.Http.HttpResults;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Common.UnifiedResponse;

namespace SoschedBack.Auth.Endpoints.Login;

public class LoginEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithSummary("Logs in user and returns user data")
        .WithRequestValidation<Request>();

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        ITokenHandlerService tokenHandlerService,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        var result = await tokenHandlerService.Login(
            request.Email,
            request.Password,
            request.RememberMe
        );

        if (result.IsFailure || result.Data == null)
        {
            var error = Error.From("Unauthorized", "UNAUTHORIZED");
            return TypedResults.Ok(Result.Failure<Response>(error));
        }

        var response = new Response(
            result.Data.Id
        );
        
        return TypedResults.Ok(Result.Success(response));
    }
    
    public sealed record Request(
        string Email,
        string Password,
        bool RememberMe
    );
    
    private record Response(int Id);
}