using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.CreateTagTypes;

public class CreateTagTypesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new tag type.")
        .WithRequestValidation<Request>();
    
    public sealed record Request(
        string Name
    );
    
    private sealed record Response(
        int Id,
        string Name
    );

    private static async Task<IResult> Handle(
        Request request,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        var tagType = new TagType
        {
            Name = request.Name.Trim(),
        };

        database.TagTypes.Add(tagType);
        await database.SaveChangesAsync(cancellationToken);
        
        var response = new Response(tagType.Id, tagType.Name);
        
        var result = Result.Success(response);
        
        return Results.Ok(result);
    }
}