using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Storage;
using SoschedBack.Storage.Seeding;

namespace SoschedBack;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        } 
        else
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                    if (exceptionHandlerPathFeature?.Error is not null)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
                    }
                });
            });
        }

        app.UseHttpsRedirection();

        app.MapEndpoints();

        await SeedDatabase(app);

        await app.EnsureDatabaseCreated();
    }
    
    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SoschedBackDbContext>();

        try
        {
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    static async Task SeedDatabase(IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<SoschedBackDbContext>();
            
            var basePath = Directory.GetCurrentDirectory();
            var seedFilePath = Path.GetFullPath(
                Path.Combine(basePath, "../SoschedBack.Storage/Seeding/FakeDB.json")
            );

            await DataSeeder.SeedAsync(dbContext, seedFilePath);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Произошла ошибка во время сидинга базы данных.");
        }
    }
}