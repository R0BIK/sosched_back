using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.UpdateSpaceUsers;

public class UpdateSpaceUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPatch("/", Handle)
            .WithRequestValidation<RequestBody>()
            .WithSummary("Updates users inside the current space");
    }

    private static async Task<Ok<Result<string>>> Handle(
        [FromBody] RequestBody body,
        SoschedBackDbContext db,
        ISpaceProvider spaceProvider,
        CancellationToken ct)
    {
        var spaceId = spaceProvider.GetSpace();
        
        if (body.UsersToAdd is not null)
        {
            await AddUsers(spaceId, body.UsersToAdd, db, ct);
        }

        if (body.UsersToRemove is not null)
        {
            await RemoveUsers(spaceId, body.UsersToRemove, db, ct);
        }

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success("Users updated successfully"));
    }
    
    private static async Task AddUsers(
        int spaceId,
        List<string> userEmails,
        SoschedBackDbContext db,
        CancellationToken cancellationToken)
    {
        var existingUserEmails = await db.SpaceUsers
            .Where(su => su.SpaceId == spaceId)
            .Select(su => su.User.Email)
            .ToListAsync(cancellationToken);

        var newUserEmails = userEmails
            .Where(email => !existingUserEmails.Contains(email))
            .ToList();

        if (newUserEmails.Count == 0)
            return;

        var newUserIds = await db.Users
            .Where(u => newUserEmails.Contains(u.Email))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (newUserIds.Count == 0)
            return;

        var defaultRoleId = await db.Roles
            .Where(r => r.SpaceId == spaceId && r.Name == BaseRoleConstants.Guest)
            .Select(r => r.Id)
            .FirstAsync(cancellationToken);

        var newSpaceUsers = newUserIds.Select(userId => new SpaceUser
        {
            UserId = userId,
            SpaceId = spaceId,
            RoleId = defaultRoleId
        }).ToList();

        await db.SpaceUsers.AddRangeAsync(newSpaceUsers, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
    
    private static async Task RemoveUsers(
        int spaceId,
        List<int> userIds,
        SoschedBackDbContext db,
        CancellationToken cancellationToken)
    {
        var spaceUsersToRemove = await db.SpaceUsers
            .Where(su => su.SpaceId == spaceId && userIds.Contains(su.UserId))
            .ToListAsync(cancellationToken);

        if (!spaceUsersToRemove.Any())
            return;

        db.SpaceUsers.RemoveRange(spaceUsersToRemove);
        await db.SaveChangesAsync(cancellationToken);
    }

    public sealed record RequestBody(
        [property: JsonPropertyName("add")] List<string>? UsersToAdd,
        [property: JsonPropertyName("remove")] List<int>? UsersToRemove
    );
}