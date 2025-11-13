using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.TagTypes.Endpoints.GetTagTypes;

public class GetTagTypesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of tag types.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10
    ) : IPagedRequest;
    
    private sealed record Response(
        int Id,
        string Name,
        int TagsCount,
        int UsersCount
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var tagTypes = await database.TagTypes
            .AsNoTracking()
            .Where(i => i.SpaceId == spaceId)
            .Select(tt => new Response(
                tt.Id,
                tt.Name,
                database.Tags.Count(t => t.SpaceId == spaceId && t.TagTypeId == tt.Id),
                database.TagToSpaceUsers.Count(tsu => tsu.Tag.TagTypeId == tt.Id)
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(tagTypes);
        
        return TypedResults.Ok(result);
    }
}