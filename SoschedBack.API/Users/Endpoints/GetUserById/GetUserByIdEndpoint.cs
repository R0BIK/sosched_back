using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.GetUserById;

public class GetUserByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns a user by id with role in current space")
        .WithRequestValidation<Request>();

    public sealed record Request(int Id);

    private sealed record Response(
        int Id,
        string FirstName,
        string LastName,
        string? Patronymic,
        string Email,
        DateOnly? Birthday,
        string IconPath,
        string Role
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();

        var user = await database.Users
            .AsNoTracking()
            .Include(u => u.SpaceUsers)
                .ThenInclude(su => su.Role)
            .FirstOrDefaultAsync(u => u.Id == request.Id, ct);

        var roleName = user.SpaceUsers
            .First(su => su.SpaceId == spaceId).Role.Name;

        var response = new Response(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Patronymic,
            user.Email,
            user.Birthday,
            user.IconPath,
            roleName
        );

        return TypedResults.Ok(Result.Success(response));
    }
}