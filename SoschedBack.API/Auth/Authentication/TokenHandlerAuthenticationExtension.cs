using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using SoschedBack.Core.Common.Interfaces.Services;

namespace SoschedBack.Auth.Authentication;

public static class TokenHandlerAuthenticationExtensions
{
    public static void AddTokenHandlerAuthentication(this WebApplicationBuilder builder)
    {
        // Configure Data Protection for cookie encryption
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("./keys"))
            .SetApplicationName("Sosched-TokenHandler");

        // Configure Cookie Authentication (replaces JWT)
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "sosched-auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                //TODO: Change to Strict
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Path = "/"; 
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                
                // Handle unauthorized requests
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Authentication required\"}");
                };
                
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Access denied\"}");
                };
            });

        // Register Token Handler services
        builder.Services.AddScoped<ITokenHandlerService, TokenHandlerService>();
    }
}