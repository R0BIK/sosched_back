using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.UpdateTagUsers;

public class UpdateTagUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPatch("/{tagId:int}/users", Handle)
            .WithRequestValidation<RequestParameters>()
            .WithRequestValidation<RequestBody>()
            .WithSummary("Adds or removes space users in a tag within the current space");
    }

    private static async Task<Ok<Result<string>>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext db,
        CancellationToken ct)
    {
        var spaceId = spaceProvider.GetSpace();
        var tagId = parameters.TagId;
        
        await AddUsersHandler(spaceId, tagId, body.UsersToAdd, body.TagsToAddUsersFrom, db, ct);

        if (body.UsersToRemove is { Count: > 0 })
        {
            await RemoveUsers(spaceId, tagId, body.UsersToRemove, db, ct);
        }

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success("Tag users updated successfully"));
    }

    private static async Task AddUsersHandler(
        int spaceId,
        int targetTagId,
        List<int>? usersToAdd,
        List<int>? tagsToAddUsersFrom,
        SoschedBackDbContext db,
        CancellationToken ct)
    {

        var spaceUserIdsFromList = new List<int>();
        if (usersToAdd is { Count: > 0 })
        {
            spaceUserIdsFromList = await db.SpaceUsers
                .AsNoTracking()
                .Where(su => su.SpaceId == spaceId && usersToAdd.Contains(su.UserId))
                .Select(su => su.Id)
                .ToListAsync(ct);
        }

        var spaceUserIdsFromTags = new List<int>();
        if (tagsToAddUsersFrom is { Count: > 0 })
        {
            spaceUserIdsFromTags = await db.TagToSpaceUsers
                .AsNoTracking()
                .Where(tsu => tagsToAddUsersFrom.Contains(tsu.TagId))
                .Select(tsu => tsu.SpaceUserId)
                .Distinct()
                .ToListAsync(ct);
        }
        
        var allSpaceUserIdsToAdd = new HashSet<int>(spaceUserIdsFromList);
        allSpaceUserIdsToAdd.UnionWith(spaceUserIdsFromTags);

        if (allSpaceUserIdsToAdd.Count == 0)
        {
            return;
        }

        
        var existingSpaceUserIds = await db.TagToSpaceUsers
            .AsNoTracking()
            .Where(tsu => tsu.TagId == targetTagId && allSpaceUserIdsToAdd.Contains(tsu.SpaceUserId))
            .Select(tsu => tsu.SpaceUserId)
            .ToListAsync(ct);
        
        var existingSet = new HashSet<int>(existingSpaceUserIds);

        var newRelations = allSpaceUserIdsToAdd
            .Where(id => !existingSet.Contains(id))
            .Select(id => new TagToSpaceUser
            {
                TagId = targetTagId,
                SpaceUserId = id
            })
            .ToList();

        if (newRelations.Count > 0)
        {
            await db.TagToSpaceUsers.AddRangeAsync(newRelations, ct);
        }
    }

    private static async Task RemoveUsers(
        int spaceId,
        int tagId,
        List<int> userIds,
        SoschedBackDbContext db,
        CancellationToken ct)
    {
        var spaceUserIds = await db.SpaceUsers
            .AsNoTracking()
            .Where(su => su.SpaceId == spaceId && userIds.Contains(su.UserId))
            .Select(su => su.Id)
            .ToListAsync(ct);

        if (spaceUserIds.Count == 0)
            return;

        var relationsToRemove = await db.TagToSpaceUsers
            .AsNoTracking()
            .Where(tsu => tsu.TagId == tagId && spaceUserIds.Contains(tsu.SpaceUserId))
            .ToListAsync(ct);

        if (relationsToRemove.Count > 0)
        {
            db.TagToSpaceUsers.RemoveRange(relationsToRemove);
        }
    }

    public sealed record RequestParameters(int TagId);

    public sealed record RequestBody(
        [property: JsonPropertyName("add")] List<int>? UsersToAdd,
        [property: JsonPropertyName("remove")] List<int>? UsersToRemove,
        [property: JsonPropertyName("addFromTags")] List<int>? TagsToAddUsersFrom
    );
}