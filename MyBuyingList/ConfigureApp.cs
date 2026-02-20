using Microsoft.EntityFrameworkCore;
using MyBuyingList.Infrastructure;
using MyBuyingList.Infrastructure.Persistence.Seeders;
using MyBuyingList.Web.Middlewares;

namespace MyBuyingList.Web;

internal static class ConfigureApp
{
    internal static async Task StartApplication(this WebApplication app)
    {
        //if (app.Environment.IsDevelopment()) app.AddDebuggingMiddlewareLogic();

        try
        {
            await app.RunDatabaseMigrations();
        }
        catch (Exception ex)
        {
            app.Logger.LogError("Failed running migrations. Err: {ExMessage}, Exception: {ExInnerException}", ex.Message, ex.InnerException);
            await app.StopAsync();
        }

        app.Logger.LogInformation("Migration ran successfully");

        app.AddMiddlewares();
    }

    private static async Task RunDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
        await seeder.SeedAsync();
    }

    private static void AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseRouting();
        app.UseRateLimiter();
        app.AddSwagger();
        app.UseAuthentication();
        app.UseAuthorization();
        app.EnableRequestBuffering();
        app.MapControllers();
    }

    private static void AddSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }

    private static void EnableRequestBuffering(this WebApplication app)
    {
        // Needed for RequestBodyValidationFilter, so it can access the request body more than one time for doing the validation.
        // TODO: investigate performance issues
        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });
    }
}
