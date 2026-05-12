using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Filtration;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Common.Requests;
using SoschedBack.Common.Responses;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
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
        bool Descending = false,
        string? Filter = null
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
        
        var filters = FilterParser.Parse(request.Filter);

        var query = ApplyFilters(spaceId, filters, database);
        
        var tagsQuery = BuildSortedUsersCountQuery(request, query, spaceId, database);
        
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
        IQueryable<Tag> baseQuery,
        int spaceId,
        SoschedBackDbContext dbContext)
    {
        var query = baseQuery
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
    
    private static IQueryable<Tag> ApplyFilters(
        int spaceId,
        ParsedFilter filters,
        SoschedBackDbContext dbContext
    )
    {
        var baseQuery = dbContext.Tags
            .AsNoTracking()
            .Where(u => u.SpaceId == spaceId);
        
        if (filters.Has(FilterConstants.TagTypeKey))
        {
            var tagTypes = filters.GetIntValues(FilterConstants.TagTypeKey);
            
            baseQuery = baseQuery
                .Where(u => tagTypes.Contains(u.TagType.Id));
        }

        return baseQuery;
    }
}

