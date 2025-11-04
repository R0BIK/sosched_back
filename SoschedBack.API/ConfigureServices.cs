using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Auth.Authentication;
using SoschedBack.Common.Filters;
using SoschedBack.Common.Http;
using SoschedBack.Core.Common.Interfaces.Services;
using SoschedBack.Core.Models;
using SoschedBack.Storage;

namespace SoschedBack;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSwagger();
        builder.AddDatabase();
        builder.AddTokenHandlerAuthentication();
        
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ITokenHandlerService, TokenHandlerService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IUserProvider, UserProvider>();
        builder.Services.AddScoped<ISpaceProvider, SpaceProvider>();
        builder.Services.AddScoped<ExtractSpaceDomainFilter>();
        builder.Services.AddScoped<UserContextFilter>();
        
        // builder.AddJwtAuthentication();
        // builder.AddAuthorization();
        //
        // builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
        //
        // builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        // builder.Services.AddScoped<IUserService, UserService>();
        
        builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
    }

    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);
        });
    }
    
    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Local")
                               ?? throw new InvalidOperationException("Connection string 'Local' not found.");

        connectionString = connectionString
            .Replace("${POSTGRES_HOST}", Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost")
            .Replace("${POSTGRES_DB}", Environment.GetEnvironmentVariable("POSTGRES_DB") ?? throw new InvalidOperationException("POSTGRES_DB environment variable not set"))
            .Replace("${POSTGRES_USER}", Environment.GetEnvironmentVariable("POSTGRES_USER") ?? throw new InvalidOperationException("POSTGRES_USER environment variable not set"))
            .Replace("${POSTGRES_PASSWORD}", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? throw new InvalidOperationException("POSTGRES_PASSWORD environment variable not set"));

        builder.Services.AddDbContext<SoschedBackDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }
}