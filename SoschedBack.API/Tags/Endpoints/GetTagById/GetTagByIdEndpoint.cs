using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.GetTagById;

public class GetTagByIdEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tags.");

    public sealed record Request(
        int Id
    );

    public sealed record Response(
        int Id,
        int TagType,
        string Name,
        string ShortName,
        string Color
    );

    private static async Task<IResult> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var tag = await database.Tags
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        if (tag == null)
        {
            var error = Error.From(
                $"Tag with id {request.Id} does not exist.",
                "ENTITY_DOES_NOT_EXISTS"
            );
            
            return Results.NotFound(error);
        }

        var response = new Response(
            tag.Id,
            tag.TagTypeId,
            tag.Name,
            tag.ShortName,
            tag.Color
        );
        
        var result = Result.Success(response);
        
        return Results.Ok(result);
    }
}