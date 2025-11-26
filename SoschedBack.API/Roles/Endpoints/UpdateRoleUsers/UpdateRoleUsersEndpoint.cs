using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Roles.Endpoints.UpdateRoleUsers;

public class UpdateRoleUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPatch("/{roleId:int}/users", Handle)
            .WithRequestValidation<RequestParameters>()
            .WithRequestValidation<RequestBody>()
            .WithSummary("Updates the role for multiple users in the current space.");
    }

    public sealed record RequestParameters(int RoleId);

    public sealed record RequestBody(
        [property: JsonPropertyName("users")] List<int>? Users, 
        [property: JsonPropertyName("tags")] List<int>? Tags
    );

    private static async Task<Ok<Result<string>>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext db,
        CancellationToken ct)
    {
        var spaceId = spaceProvider.GetSpace();
        var targetRoleId = parameters.RoleId;

        var roleExists = await db.Roles
            .AsNoTracking()
            .AnyAsync(r => r.Id == targetRoleId && r.SpaceId == spaceId, ct);
        
        if (!roleExists)
        {
            return TypedResults.Ok(Result.Failure<string>(
                Error.From($"Role with ID {targetRoleId} not found in current space.", "ROLE_NOT_FOUND")));
        }

        var spaceUserIdsToUpdate = await CollectSpaceUserIds(spaceId, body.Users, body.Tags, db, ct);

        if (spaceUserIdsToUpdate.Count == 0)
        {
            return TypedResults.Ok(Result.Success("No valid users or tags found for update."));
        }
        
        var spaceUsers = await db.SpaceUsers
            .Where(su => su.SpaceId == spaceId && spaceUserIdsToUpdate.Contains(su.Id))
            .ToListAsync(ct);
        
        foreach (var spaceUser in spaceUsers)
        {
            spaceUser.RoleId = targetRoleId;
        }

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success($"Successfully updated role for {spaceUsers.Count} user(s)."));
    }

    /// <summary>
    /// Собирает уникальный список SpaceUser ID из прямого списка User ID и Tag ID.
    /// </summary>
    private static async Task<HashSet<int>> CollectSpaceUserIds(
        int spaceId,
        List<int>? userIds,
        List<int>? tagIds,
        SoschedBackDbContext db,
        CancellationToken ct)
    {
        var collectedIds = new HashSet<int>();

        // 1. Сбор SpaceUser ID из прямого списка Global User ID
        if (userIds is { Count: > 0 })
        {
            var spaceUserIds = await db.SpaceUsers
                .AsNoTracking()
                .Where(su => su.SpaceId == spaceId && userIds.Contains(su.UserId))
                .Select(su => su.Id)
                .ToListAsync(ct);
            
            collectedIds.UnionWith(spaceUserIds);
        }

        // 2. Сбор SpaceUser ID из Тегов
        if (tagIds is { Count: > 0 })
        {
            var spaceUserIdsFromTags = await db.TagToSpaceUsers
                .AsNoTracking()
                .Where(tsu => tagIds.Contains(tsu.TagId))
                .Select(tsu => tsu.SpaceUserId)
                .Distinct()
                .ToListAsync(ct);
            
            collectedIds.UnionWith(spaceUserIdsFromTags);
        }

        return collectedIds;
    }
}