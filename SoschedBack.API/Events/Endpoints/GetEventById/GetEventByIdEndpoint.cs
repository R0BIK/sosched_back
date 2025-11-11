using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Events.Endpoints.GetEventById;

public class GetEventByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns an event by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    private sealed record Response(
        int Id,
        string Name,
        string? Location,
        string? Description,
        int CreatorId,
        string Color,
        DateTimeOffset DateStart,
        DateTimeOffset DateEnd,
        int EventTypeId,
        int? CoordinatorId
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var myEvent = await database.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        var response = new Response(
            myEvent.Id,
            myEvent.Name,
            myEvent.Location,
            myEvent.Description,
            myEvent.CreatorId,
            myEvent.Color,
            myEvent.DateStart,
            myEvent.DateEnd,
            myEvent.EventTypeId,
            myEvent.CoordinatorId
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}