using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;

namespace SoschedBack.Core.Common.Interfaces.Services;

public interface ITokenHandlerService
{
    Task<Result<User>> Login(string login, string password, bool rememberMe);
    Task Logout();
    Task<Result<User>> GetCurrentUser();
}