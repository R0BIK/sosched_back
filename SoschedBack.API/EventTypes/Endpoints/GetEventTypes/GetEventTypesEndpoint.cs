using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.EventTypes.Endpoints.GetEventTypes;

public class GetEventTypesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of event types.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10
    ) : IPagedRequest;
    
    private sealed record Response(
        int Id,
        string Name
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var eventTypes = await database.EventTypes
            .AsNoTracking()
            .Select(eventType => new Response(
                eventType.Id,
                eventType.Name
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(eventTypes);
        
        return TypedResults.Ok(result);
    }
}