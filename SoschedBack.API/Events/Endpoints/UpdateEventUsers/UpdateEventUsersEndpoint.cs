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

namespace SoschedBack.Events.Endpoints.UpdateEventUsers;

public class UpdateEventUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app)
    {
        return app
            .MapPatch("/{eventId:int}/users", Handle)
            .WithRequestValidation<RequestParameters>()
            .WithRequestValidation<RequestBody>()
            .WithSummary("Adds or removes space users in an event within the current space");
    }

    private static async Task<Ok<Result<string>>> Handle(
        [AsParameters] RequestParameters parameters,
        [FromBody] RequestBody body,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext db,
        CancellationToken ct)
    {
        var spaceId = spaceProvider.GetSpace();
        var eventId = parameters.EventId;
        
        await AddUsersHandler(spaceId, eventId, body.UsersToAdd, body.TagsToAddUsersFrom, db, ct);

        if (body.UsersToRemove is { Count: > 0 })
        {
            await RemoveUsers(spaceId, eventId, body.UsersToRemove, db, ct);
        }

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(Result.Success("Event users updated successfully"));
    }
    
    private static async Task AddUsersHandler(
        int spaceId,
        int targetEventId,
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
        
        var existingSpaceUserIds = await db.EventToSpaceUsers
            .AsNoTracking()
            .Where(esu => esu.EventId == targetEventId && allSpaceUserIdsToAdd.Contains(esu.SpaceUserId))
            .Select(esu => esu.SpaceUserId)
            .ToListAsync(ct);
        
        var existingSet = new HashSet<int>(existingSpaceUserIds);

        var newRelations = allSpaceUserIdsToAdd
            .Where(id => !existingSet.Contains(id))
            .Select(id => new EventToSpaceUser 
            {
                EventId = targetEventId,
                SpaceUserId = id
            })
            .ToList();

        if (newRelations.Count > 0)
        {
            await db.EventToSpaceUsers.AddRangeAsync(newRelations, ct);
        }
    }
    
    private static async Task RemoveUsers(
        int spaceId,
        int eventId,
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
        
        var relationsToRemove = await db.EventToSpaceUsers
            .Where(esu => esu.EventId == eventId && spaceUserIds.Contains(esu.SpaceUserId))
            .ToListAsync(ct);

        if (relationsToRemove.Count > 0)
        {
            db.EventToSpaceUsers.RemoveRange(relationsToRemove);
        }
    }

    public sealed record RequestParameters(int EventId); 

    public sealed record RequestBody(
        [property: JsonPropertyName("add")] List<int>? UsersToAdd,
        [property: JsonPropertyName("remove")] List<int>? UsersToRemove,
        [property: JsonPropertyName("addFromTags")] List<int>? TagsToAddUsersFrom
    );
}