using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SoschedBack.Auth.Endpoints.Login;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Auth.Endpoints.Register;

public class RegisterEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPost("/register", Handle)
            .WithSummary("Registers a new user")
            .WithRequestValidation<Request>();
    }
    
    public sealed record Request(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );

    private sealed record Response(
        int Id
    );

    private static async Task<Ok<Result<Response>>> Handle(
        Request request,
        IPasswordHasher<User> passwordHasher,
        ITokenHandlerService tokenHandlerService,
        SoschedBackDbContext db,
        CancellationToken cancellationToken
    )
    {
        var email = request.Email.Trim().ToLower();
        
        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Password = passwordHasher.HashPassword(null!, request.Password),
            IconPath = "/default-user-icon.png"
        };
        
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        var loginResult = await tokenHandlerService.Login(email, request.Password, rememberMe: true);
        if (loginResult.IsFailure)
        {
            return TypedResults.Ok(Result.Failure<Response>(
                Error.From("Registration succeeded, but login failed.", "LOGIN_AFTER_REGISTER_FAILED")));
        }

        var response = new Response(user.Id);
        return TypedResults.Ok(Result.Success(response));
    }
}