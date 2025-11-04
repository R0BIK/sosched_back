using SoschedBack.Core.Models;

namespace SoschedBack.Common.Http;

public class UserProvider : IUserProvider
{
    IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public User GetUser()
    {
        var context = _httpContextAccessor.HttpContext;

        return context!.GetCurrentUser() ?? throw new InvalidOperationException("Current user is not set in HttpContext");
    }
}