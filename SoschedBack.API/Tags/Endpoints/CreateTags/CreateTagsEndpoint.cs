using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.CreateTags;

public class CreateTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new tag");
    
    public sealed record Request(
        string Name,
        string ShortName,
        string Color,
        int TagType
    );
    
    private sealed record Response(
        int Id,
        string Name,
        string ShortName,
        string Color,
        int TagType
    );

    private static async Task<IResult> Handle(
        Request request,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        if (await IsTagNameExists(request.Name, database, cancellationToken)) 
            return Results.BadRequest($"Tag with '{request.Name}' name already exists");
        
        if (await IsTagShortNameExists(request.ShortName, database, cancellationToken))
            return Results.BadRequest($"Tag with '{request.ShortName}' short name already exists");
        
        var tagType = await GetTagType(request.TagType, database, cancellationToken);
        if (tagType == null)
        {
            return Results.BadRequest($"Tag type with id {request.TagType} not found");
        }

        var tag = new Tag
        {
            Name = request.Name,
            ShortName = request.ShortName,
            Color = request.Color,
            TagTypeId = tagType.Id,
        };

        try
        {
            database.Tags.Add(tag);
            await database.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return Results.StatusCode(500);
        }
        
        var response = new Response(tag.Id, tag.Name, tag.ShortName, tag.Color, tag.TagTypeId);
        return Results.Ok(response);
    }

    private static async Task<TagType?> GetTagType(
        int id,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        return await database.TagTypes.FindAsync( [id], cancellationToken);
    }

    private static async Task<bool> IsTagNameExists(
        string name,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        return await database.Tags.AnyAsync(r => r.Name == name, cancellationToken);
    }
    
    private static async Task<bool> IsTagShortNameExists(
        string shortName,
        SoschedBackDbContext database,
        CancellationToken cancellationToken
    )
    {
        return await database.Tags.AnyAsync(r => r.ShortName == shortName, cancellationToken);
    }
}