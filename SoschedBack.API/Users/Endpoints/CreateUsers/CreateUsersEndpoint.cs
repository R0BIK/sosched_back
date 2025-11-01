using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.CreateUsers;

public class CreateUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new user.")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        string FirstName,
        string LastName,
        string Patronymic,
        string Email,
        string Password
    );
    
    private sealed record Response(
        int Id,
        string FirstName,
        string LastName,
        string Patronymic,
        string Email,
        string Password
    );

    private static async Task<IResult> Handle(
        Request request,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Patronymic = request.Patronymic.Trim(),
            Email = request.Email.Trim(),
            Password = request.Password.Trim()
        };

        database.Users.Add(user);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(
            user.Id, 
            user.FirstName,
            user.LastName,
            user.Patronymic,
            user.Email,
            user.Password
        );
        
        var result = Result.Success(response);
        
        return Results.Ok(result);
    }
}