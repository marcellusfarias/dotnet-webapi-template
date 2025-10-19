using Microsoft.EntityFrameworkCore;
using MyBuyingList.Infrastructure;
using MyBuyingList.Web.Middlewares;
using System.Diagnostics;
using System.Reflection;

namespace MyBuyingList.Web;

internal static class ConfigureApp
{
    internal static async Task StartApplication(this WebApplication app)
    {
        //if (app.Environment.IsDevelopment()) app.AddDebuggingMiddlewareLogic();

        try
        {
            app.RunDatabaseMigrations();
        }
        catch (Exception ex)
        {
            app.Logger.LogError("Failed running migrations. Err: {ExMessage}, Exception: {ExInnerException}", ex.Message, ex.InnerException);
            await app.StopAsync();
        }

        app.Logger.LogInformation("Migration ran successfully");

        app.AddMiddlewares();
    }

    private static void RunDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = (ApplicationDbContext)scope.ServiceProvider.GetRequiredService(typeof(ApplicationDbContext));
        db.Database.Migrate();
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

    #region UsefulFunctions

    // Call this on the StartApplication method if you want to log the custom added middlewares.
    private static void PrintListOfCustomMiddlewares(this WebApplication app)
    {
        FieldInfo applicationBuilderFieldInfo = app.GetType()
                            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                            .Single(pi => pi.Name == "<ApplicationBuilder>k__BackingField");

        object? applicationBuilderValue = applicationBuilderFieldInfo.GetValue(app);
        if (applicationBuilderValue is null)
        {
            return;
        }
        
        Type appBuilderType = applicationBuilderValue.GetType();

        // Get the FieldInfo for the private field "_components" in ApplicationBuilder
        FieldInfo? componentsField = appBuilderType.GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);

        if (componentsField == null)
        {
            return;
        }
            
        List<Func<RequestDelegate, RequestDelegate>>? componentsValue =
            componentsField.GetValue(applicationBuilderValue) as List<Func<RequestDelegate, RequestDelegate>>;

        if (componentsValue != null)
        {
            foreach (var func in componentsValue)
            {
                var target = func.Target;

                if (target == null)
                {
                    continue;
                }
                        
                Type targetType = target.GetType();
                FieldInfo? stateFieldInfo = targetType.GetField("state");

                if (stateFieldInfo == null)
                {
                    continue;
                }
                        
                var stateValue = stateFieldInfo.GetValue(target);

                if (stateValue == null)
                {
                    continue;
                }
                            
                Type stateType = stateValue.GetType();
                PropertyInfo middlewareFieldInfo = stateType.GetProperty("Middleware")!;
                object? middlewareValue = middlewareFieldInfo.GetValue(stateValue);

                if (middlewareValue == null)
                {
                    continue;
                }
                        
                Type middlewareType = middlewareValue.GetType();
                PropertyInfo nameInfo = middlewareType.GetProperty("Name")!;
                object nameValue = nameInfo.GetValue(middlewareValue)!;
                app.Logger.LogInformation("Middleware: {NameValue}", nameValue);
            }
        }
    }

    private static void AddDebuggingMiddlewareLogic(this WebApplication app)
    {
        // Grab the "Microsoft.AspNetCore" DiagnosticListener from DI
        var listener = app.Services.GetRequiredService<DiagnosticListener>();

        // Create an instance of the AnalysisDiagnosticAdapter using the IServiceProvider
        // so that the ILogger is injected from DI
        var observer = ActivatorUtilities.CreateInstance<AnalysisDiagnosticAdapter>(app.Services);

        // Subscribe to the listener with the SubscribeWithAdapter() extension method
        //using var disposable = 
        listener.SubscribeWithAdapter(observer);
    }

    #endregion
}
