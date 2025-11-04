namespace SoschedBack.Common.Http;

public class SpaceProvider : ISpaceProvider
{
    IHttpContextAccessor _httpContextAccessor;

    public SpaceProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public int GetSpace()
    {
        var context = _httpContextAccessor.HttpContext;

        return context!.GetCurrentSpaceId() ?? throw new InvalidOperationException("Current space id is not set in HttpContext");
    }
}