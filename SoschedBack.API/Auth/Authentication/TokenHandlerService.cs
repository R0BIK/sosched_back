using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Auth.Authorization;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Common.UnifiedResponse;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack.Auth.Authentication;

public class TokenHandlerService : ITokenHandlerService
{
    private readonly SoschedBackDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public TokenHandlerService(
        SoschedBackDbContext context,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
    }
 
    public async Task<Result<User>> Login(string login, string password, bool rememberMe)
    {
        var user = await FindUser(login);
        if (user == null || !ValidatePassword(user, password))
        {
            var error = Error.From(
                "Invalid login or password.",
                "INVALID_CREDENTIALS"
            );
            return Result.Failure<User>(error);
        }

        var claims = BuildClaims(user);
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe 
                ? DateTime.UtcNow.AddDays(7) 
                : DateTime.UtcNow.AddHours(2)
        };

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            claimsPrincipal, 
            authProperties);

        return Result.Success(user);
    }
    
    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<Result<User>> GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return Result.Failure<User>(
                Error.From("User is not authenticated.", "USER_NOT_AUTHENTICATED")
            );

        if (!httpContext.User.TryGetUserId(out var userId))
            return Result.Failure<User>(
                Error.From("User ID is missing or invalid in the current user's claims.", "CLAIM_ID_MISSING")
            );

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Result.Failure<User>(
                Error.From($"User with ID '{userId}' does not exist.", "ENTITY_DOES_NOT_EXIST")
            );
        
        return Result.Success(user);
    }

    private async Task<User?> FindUser(string login)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == login);
    }

    private bool ValidatePassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        return result == PasswordVerificationResult.Success;
    }

    private static List<Claim> BuildClaims(User user)
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };
    }
}