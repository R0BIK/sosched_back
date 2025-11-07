using System.Linq.Dynamic.Core;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Common;
using SoschedBack.Common.Constants;
using SoschedBack.Common.Extensions;
using SoschedBack.Common.Filtration;
using SoschedBack.Common.Http;
using SoschedBack.Common.Pagination;
using SoschedBack.Common.Pagination.PagedRequest;
using SoschedBack.Common.Requests;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Users.Endpoints.GetUsers;

public class GetUsersEndpoint : IEndpoint
{
    public static IEndpointConventionBuilder Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Returns a list of users.")
        .WithRequestValidation<Request>();

    public sealed record Request(
        int? Page = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool Descending = false,
        string? Filter = null
    ) : IPagedRequest, ISortRequest;
    
    private sealed record Response(
        int Id,
        string FirstName,
        string LastName,
        string? Patronymic,
        string Email,
        string IconPath
    );

    private static async Task<Ok<Result<PagedList<Response>>>> Handle(
        [AsParameters] Request request,
        ISpaceProvider spaceProvider,
        SoschedBackDbContext database,
        CancellationToken ct
    )
    {
        var spaceId = spaceProvider.GetSpace();
        
        var filters = FilterParser.Parse(request.Filter);

        var query = ApplyFilters(spaceId, filters, database);
        
        //TODO: Apply sorting
        
        var users = await query
            .AsNoTracking()
            .Select(user => new Response(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Patronymic,
                user.Email,
                user.IconPath
            ))
            .ToPagedListAsync(request, ct);
        
        var result = Result.Success(users);
        
        return TypedResults.Ok(result);
    }

    private static IQueryable<User> ApplyFilters(
        int spaceId,
        ParsedFilter filters,
        SoschedBackDbContext dbContext
    )
    {
        var baseQuery = dbContext.Users
            .AsNoTracking()
            .Where(u => u.SpaceUsers.Any(su => su.SpaceId == spaceId));
        
        if (filters.Has(FilterConstants.RoleKey))
        {
            var roles = filters.GetValues(FilterConstants.RoleKey);
            baseQuery = baseQuery
                .Where(u => u.SpaceUsers.Any(su => roles.Contains(su.Role.Name)));
        }
        
        foreach (var key in filters.Keys.Where(k => k.StartsWith(FilterConstants.TagTypePrefix, StringComparison.OrdinalIgnoreCase)))
        {
            var tagTypeName = key[FilterConstants.TagTypePrefix.Length..];
            var tagNames = filters.GetValues(key);

            baseQuery = baseQuery.
                Where(u => 
                    u.TagToUsers.Any(tu =>
                    tu.Tag.TagType.Name == tagTypeName &&
                    tagNames.Contains(tu.Tag.Name)));
        }

        return baseQuery;
    }
}