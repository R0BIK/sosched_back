using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoschedBack.Storage;

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
}