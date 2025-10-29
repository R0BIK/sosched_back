using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.GetTags;

public class GetTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tags.");

    public sealed record Response(
        int Id,
        string TagType,
        string Name,
        string ShortName,
        string Color
    );

    private static async Task<IResult> Handle(
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var tags = await database.Tags
            .Select(tag => new Response(
                tag.Id,
                tag.TagType.Name,
                tag.Name,
                tag.ShortName,
                tag.Color
            ))
            .ToListAsync(ct);
        
        var result = Result.Success(tags);
        
        return Results.Ok(result);
    }
}