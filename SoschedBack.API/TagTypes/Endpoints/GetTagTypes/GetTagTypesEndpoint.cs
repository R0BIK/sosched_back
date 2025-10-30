using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.GetTagTypes;

public class GetTagTypesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tags types.");

    public sealed record Response(
        int Id,
        string Name
    );

    private static async Task<IResult> Handle(
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var tagTypes = await database.TagTypes
            .Select(tagType => new Response(
                tagType.Id,
                tagType.Name
            ))
            .ToListAsync(ct);
        
        var result = Result.Success(tagTypes);
        
        return Results.Ok(result);
    }
}