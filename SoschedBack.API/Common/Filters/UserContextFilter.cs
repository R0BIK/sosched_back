using SoschedBack.Common.Constants;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Common.UnifiedResponse;

namespace SoschedBack.Common.Filters;

public class UserContextFilter : IEndpointFilter
{
    private readonly ITokenHandlerService _tokenHandlerService;

    public UserContextFilter(ITokenHandlerService tokenHandlerService)
    {
        _tokenHandlerService = tokenHandlerService;
    }
    
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Get current user from secure cookie
        var userResult = await _tokenHandlerService.GetCurrentUser();

        if (userResult.IsFailure || userResult.Data is null)
        {
            return TypedResults.Json(
                Result.Failure(userResult.Error),
                statusCode: StatusCodes.Status401Unauthorized
            );
        }
        
        context.HttpContext.Items[HttpContextItemKeys.CurrentUser] = userResult.Data;
        
        return await next(context);
    }
}