using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.GetTags;

public class GetTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tags.");

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest;

    public sealed record Response(
        int Id,
        string TagType,
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
        var tags = await database.Tags
            .ApplySorting(
                request.SortBy,
                request.Descending
            )
            .Select(tag => new Response(
                tag.Id,
                tag.TagType.Name,
                tag.Name,
                tag.ShortName,
                tag.Color
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(tags);
        
        return Results.Ok(result);
    }
}