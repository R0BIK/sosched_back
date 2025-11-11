using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Common.Requests;
using SoschedBack.Common.Responses;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.GetTags;

public class GetTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tags.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest, ISortRequest;

    private sealed record Response(
        int Id,
        string TagType,
        string Name,
        string ShortName,
        string Color,
        int UsersCount
    ) : IUsersCountResponse;

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var tagsQuery = BuildSortedUsersCountQuery(request, spaceId, database);
        
        var tags = await tagsQuery
            .AsNoTracking()
            .Select(tag => new Response(
                tag.Id,
                tag.TagType,
                tag.Name,
                tag.ShortName,
                tag.Color,
                tag.UsersCount
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(tags);
        
        return TypedResults.Ok(result);
    }

    private static IQueryable<Response> BuildSortedUsersCountQuery(
        Request request, 
        int spaceId,
        SoschedBackDbContext dbContext)
    {
        var query = dbContext.Tags
            .AsNoTracking()
            .Where(t => t.SpaceId == spaceId)
            .Select(tag => new Response(
                tag.Id,
                tag.TagType.Name,
                tag.Name,
                tag.ShortName,
                tag.Color,
                dbContext.TagToSpaceUsers.Count(tu => tu.TagId == tag.Id)
            ));

        return query.ApplySorting(request.SortBy, request.Descending);
    }
}

