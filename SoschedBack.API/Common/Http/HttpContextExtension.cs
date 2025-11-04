using SoschedBack.Common.Constants;
using SoschedBack.Core.Models;

namespace SoschedBack.Common.Http;

public static class HttpContextExtension
{
    public static User? GetCurrentUser(this HttpContext context)
    {
        return context.Items[HttpContextItemKeys.CurrentUser]! as User;
    }

    public static int? GetCurrentSpaceId(this HttpContext context)
    {
        return context.Items[HttpContextItemKeys.SpaceId]! as int?;
    }
}