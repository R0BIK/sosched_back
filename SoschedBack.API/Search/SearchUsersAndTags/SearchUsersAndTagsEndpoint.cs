using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Search.SearchUsersAndTags;

public class SearchUsersAndTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Search users and tags combined.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Limit = 10, 
        string? Search = null
    );
    
    private sealed record Response(
        List<UserResult> Users,
        List<TagResult> Tags
    );
    
    private sealed record UserResult(
        int Id,
        string Title,
        string Subtitle,
        List<UserTags> UserTags 
    );
    
    private sealed record TagResult(
        int Id,
        string Title,
        string Subtitle,
        string? Color
    );
    
    private sealed record UserTags(
        int Id,
        string ShortName,
        string Color
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext db,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        var search = request.Search?.Trim() ?? "";
        var pattern = $"%{search}%";
        var limit = request.Limit ?? 10;

        var users = await db.Users
            .AsNoTracking()
            .Where(u => u.SpaceUsers.Any(su => su.SpaceId == spaceId)
                        && (
                            EF.Functions.ILike(u.FirstName, pattern) ||
                            EF.Functions.ILike(u.LastName, pattern) ||
                            EF.Functions.ILike(u.Email, pattern) ||
                            EF.Functions.ILike(u.FirstName + " " + u.LastName, pattern) ||
                            EF.Functions.ILike(u.LastName + " " + u.FirstName, pattern)
                        ))
            .OrderBy(u => u.LastName) 
            .Take(limit)
            .Select(u => new UserResult(
                u.Id,
                (u.LastName + " " + u.FirstName + " " + (u.Patronymic ?? "")).Trim(),
                u.Email,
                (from su in u.SpaceUsers
                 where su.SpaceId == spaceId
                 from tsu in su.TagToSpaceUsers
                 select new UserTags(
                     tsu.Tag.Id,
                     tsu.Tag.ShortName,
                     tsu.Tag.Color
                 )).ToList()
            ))
            .ToListAsync(ct);

        var tags = await db.Tags
            .AsNoTracking()
            .Where(t => t.SpaceId == spaceId
                        && (
                            EF.Functions.ILike(t.Name, pattern) ||
                            EF.Functions.ILike(t.ShortName, pattern)
                        ))
            .OrderBy(t => t.Name)
            .Take(limit)
            .Select(t => new TagResult(
                t.Id,
                t.Name,
                t.ShortName,
                t.Color
            ))
            .ToListAsync(ct);

        var response = new Response(users, tags);
        
        return TypedResults.Ok(Result.Success(response));
    }
}