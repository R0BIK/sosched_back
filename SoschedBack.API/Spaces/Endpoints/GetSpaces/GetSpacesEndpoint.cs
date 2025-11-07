using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Common.Requests;
using SoschedBack.Common.Responses;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Spaces.Endpoints.GetSpaces;

public class GetSpacesEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of available for user spaces.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10
    ) : IPagedRequest;

    private sealed record Response(
        int Id,
        string Name,
        string Domain
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        IUserProvider userProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var user = userProvider.GetUser();
        
        var spaces = await database.Spaces
            .AsNoTracking()
            .Where(t => t.SpaceUsers.Any(u => u.UserId == user.Id))
            .Select(space => new Response(
                space.Id,
                space.Name,
                space.Domain
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(spaces);
        
        return TypedResults.Ok(result);
    }
}