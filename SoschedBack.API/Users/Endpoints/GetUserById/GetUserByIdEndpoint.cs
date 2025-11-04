using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.GetUserById;

public class GetUserByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns a user by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    public sealed record Response(
        int Id,
        string FirstName,
        string LastName,
        string? Patronymic,
        string Email,
        DateOnly? BirthDate,
        string IconPath
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var user = await database.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        var response = new Response(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Patronymic,
            user.Email,
            user.Birthday,
            user.IconPath
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}