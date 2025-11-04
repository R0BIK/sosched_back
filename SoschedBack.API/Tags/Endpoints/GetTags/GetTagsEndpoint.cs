using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Tags.Endpoints.GetTags;

public class GetTagsEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tags.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false
    ) : IPagedRequest;

    private sealed record Response(
        int Id,
        string TagType,
        string Name,
        string ShortName,
        string Color
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var tags = await database.Tags
            .AsNoTracking()
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
        
        return TypedResults.Ok(result);
    }
}