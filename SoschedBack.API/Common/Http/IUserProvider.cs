using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;

namespace SoschedBack.Common.Http;

public interface IUserProvider
{
    User GetUser();
}