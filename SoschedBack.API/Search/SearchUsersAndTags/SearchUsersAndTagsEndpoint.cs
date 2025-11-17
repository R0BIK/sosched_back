using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
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
        int? Page = 1,
        int? PageSize = 10,
        string? Search = null
    ) : IPagedRequest;

    private sealed record Response(
        string Type,
        int Id,
        string Title,
        string Subtitle,
        string? Color
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext db,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        // var requestJson = System.Text.Json.JsonSerializer.Serialize(request);
        // Console.WriteLine("Request object: " + requestJson);

        var search = request.Search?.Trim() ?? "";
        var pattern = $"%{search}%";
        
        Console.WriteLine("inputSearch " + pattern);

        var userQuery =
            from u in db.Users
            where u.SpaceUsers.Any(su => su.SpaceId == spaceId)
                  && (
                      EF.Functions.ILike(u.FirstName, pattern) ||
                      EF.Functions.ILike(u.LastName, pattern) ||
                      EF.Functions.ILike(u.Patronymic ?? "", pattern) ||
                      EF.Functions.ILike(u.Email, pattern) ||
                      EF.Functions.ILike(u.FirstName + " " + u.LastName, pattern) ||
                      EF.Functions.ILike(u.LastName + " " + u.FirstName, pattern)
                  )
            select new
            {
                Type = "user",
                Id = u.Id,
                Title = (u.LastName + " " + u.FirstName + " " + (u.Patronymic ?? "")).Trim(),
                Subtitle = u.Email,
                Color = ""
            };

        var tagQuery =
            from t in db.Tags
            where t.SpaceId == spaceId
                  && (
                      EF.Functions.ILike(t.Name, pattern) ||
                      EF.Functions.ILike(t.ShortName, pattern)
                  )
            select new
            {
                Type = "tag",
                Id = t.Id,
                Title = t.Name,
                Subtitle = t.ShortName,
                Color = t.Color
            };

        var combinedQuery =
            userQuery
                .Union(tagQuery)
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Title)
                .Select(x => new Response(
                    x.Type,
                    x.Id,
                    x.Title,
                    x.Subtitle,
                    x.Color
                ));

        var paged = await combinedQuery.ToPagedListAsync(request, ct);

        return TypedResults.Ok(Result.Success(paged));
    }
}