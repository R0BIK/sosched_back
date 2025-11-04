using Microsoft.EntityFrameworkCore;
using SoschedBack.Common.Constants;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Storage;

namespace SoschedBack.Common.Filters;

public class ExtractSpaceDomainFilter : IEndpointFilter
{
    private SoschedBackDbContext _dbContext;
    
    public ExtractSpaceDomainFilter(SoschedBackDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        
        if (!httpContext.Request.RouteValues.TryGetValue("spaceDomain", out var spaceDomainObj))
            return InvalidDomainResult();
        
        var spaceDomain = spaceDomainObj?.ToString();
        if (string.IsNullOrWhiteSpace(spaceDomain))
            return InvalidDomainResult();
        
        var spaceIdResult = await GetSpaceId(spaceDomain);
        if (!spaceIdResult.IsSuccess)
            return InvalidDomainResult();
        
        httpContext.Items[HttpContextItemKeys.SpaceId] = spaceIdResult.Data;
        return await next(context);
        
        // Static local method
        static IResult InvalidDomainResult() => 
            TypedResults.Json(
                Result.Failure(Error.From("Space domain is not valid.", "SPACE_IS_NOT_VALID")),
                statusCode: StatusCodes.Status400BadRequest
            );
    }

    private async Task<Result<int>> GetSpaceId(string? spaceDomain)
    {
        var space = await _dbContext.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Domain == spaceDomain);

        if (space is null)
            return Result.Failure<int>(Error.From("Space domain is not valid.", "SPACE_IS_NOT_VALID"));
        
        return Result.Success(space.Id);
    }
}