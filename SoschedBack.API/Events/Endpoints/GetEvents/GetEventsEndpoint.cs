using System.Linq.Dynamic.Core;
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

namespace SoschedBack.Events.Endpoints.GetEvents;

public class GetEventsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of events.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        DateTimeOffset DateFrom,
        DateTimeOffset DateTo,
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false,
        string? Filter = null
    ) : IPagedRequest, ISortRequest;

    private sealed record Response(
        int Id,
        string Name,
        string? Location,
        string? Description,
        int CreatorId,
        string Color,
        DateTimeOffset DateStart,
        DateTimeOffset DateEnd,
        int? CoordinatorId,
        int UsersCount
    ) : IUsersCountResponse;

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        IUserProvider userProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        // var sortedQuery = BuildSortedUsersCountQuery(request, spaceId, database);

        var baseQuery = database.Events
            .AsNoTracking()
            .Where(i => i.SpaceId == spaceId)
            .Where(i => i.DateStart >= request.DateFrom && i.DateEnd <= request.DateTo);

        if (request.Filter is not null)
        {
            
        }

        var user = userProvider.GetUser();

        var events = await baseQuery
            .AsNoTracking()
            .Where(i => i.EventToSpaceUsers.Any(u => u.SpaceUser.UserId == user.Id))
            .Select(myEvent => new Response(
                myEvent.Id,
                myEvent.Name,
                myEvent.Location,
                myEvent.Description,
                myEvent.CreatorId,
                myEvent.Color,
                myEvent.DateStart,
                myEvent.DateEnd,
                myEvent.CoordinatorId,
                0
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(events);
        
        return TypedResults.Ok(result);
    }

    // private static IQueryable<Response> BuildSortedUsersCountQuery(
    //     Request request, 
    //     int spaceId,
    //     SoschedBackDbContext dbContext)
    // {
    //     var query = dbContext.Tags
    //         .AsNoTracking()
    //         .Where(t => t.SpaceId == spaceId)
    //         .Select(tag => new Response(
    //             tag.Id,
    //             tag.TagType.Name,
    //             tag.Name,
    //             tag.ShortName,
    //             tag.Color,
    //             dbContext.TagToUsers.Count(tu => tu.TagId == tag.Id)
    //         ));
    //
    //     return query.ApplySorting(request.SortBy, request.Descending);
    // }
    
    // private static IQueryable<Event> ApplyFilters(
    //     int spaceId,
    //     ParsedFilter filters,
    //     Request request,
    //     SoschedBackDbContext dbContext
    // )
    // {
    //     var baseQuery = dbContext.Events
    //         .AsNoTracking()
    //         .Where(i => i.SpaceId == spaceId)
    //         .Where(i => i.DateStart >= request.DateFrom && i.DateEnd <= request.DateTo);
    //     
    //     if (filters.HasIntValues(FilterConstants.UserKey))
    //     {
    //         var users = filters.GetIntValues(FilterConstants.UserKey);
    //         baseQuery = baseQuery
    //             .Where(i => i.EventToSpaceUsers.Any(u => users.Contains(u.SpaceUser.UserId)));
    //     }
    //     
    //     if (filters.Has(FilterConstants.TagKey))
    //     {
    //         var tags = filters.GetValues(FilterConstants.TagKey);
    //         baseQuery = baseQuery
    //             .Where(i => i.EventToSpaceUsers.Any(u => users.Contains(u.SpaceUser.UserId)));
    //     }
    //
    //     return baseQuery;
    // }
}

