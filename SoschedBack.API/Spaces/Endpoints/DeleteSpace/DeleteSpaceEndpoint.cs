using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.DeleteSpace;

public class DeleteSpaceEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:int}", Handle)
        .WithSummary("Deletes a space")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    private static async Task<Results<Ok<Result<string>>, NotFound, ForbidHttpResult>> Handle(
        [AsParameters] Request request,
        IUserProvider userProvider,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var user = userProvider.GetUser();

        // 1. Find the space
        var space = await database.Spaces
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (space is null)
        {
            return TypedResults.NotFound();
        }

        // 2. Check Permissions: Ensure the current user is an Admin of this space
        var isAdmin = await database.SpaceUsers
            .AsNoTracking()
            .AnyAsync(su => 
                su.SpaceId == space.Id && 
                su.UserId == user.Id && 
                su.Role.Name == BaseRoleConstants.Admin, 
                cancellationToken);

        if (!isAdmin)
        {
            return TypedResults.Forbid();
        }

        // 3. Delete the space
        // Note: This assumes Cascade Delete is configured in your DB Context 
        // to remove related Roles, SpaceUsers, etc. automatically.
        database.Spaces.Remove(space);
        await database.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(Result.Success("Space deleted successfully."));
    }
}