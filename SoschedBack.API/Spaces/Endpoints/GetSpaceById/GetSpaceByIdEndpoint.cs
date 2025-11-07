using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.GetSpaceById;

public class GetSpaceByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Returns a space by id.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int Id
    );

    private sealed record Response(
        int Id,
        string Name,
        string Domain
    );

    private static async Task<Ok<Result<Response>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var space = await database.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        var response = new Response(
            space.Id,
            space.Name,
            space.Domain
        );
        
        var result = Result.Success(response);
        
        return TypedResults.Ok(result);
    }
}