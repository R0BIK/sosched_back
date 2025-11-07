using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.EventTypes.Endpoints.GetEventTypeById;

public class GetEventTypeByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns a tag type by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    private sealed record Response(
        int Id,
        string Name
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var eventType = await database.EventTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        var response = new Response(
            eventType.Id,
            eventType.Name
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}