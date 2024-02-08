using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MyBuyingList.Infrastructure;
using MyBuyingList.Web.Middlewares;
using System.Diagnostics;
using System.Reflection;

namespace MyBuyingList.Web;

internal static class ConfigureApp
{
    internal async static Task StartApplication(this WebApplication app)
    {
        //if (app.Environment.IsDevelopment()) app.AddDebuggingMiddlewareLogic();

        try
        {
            app.RunDatabaseMigrations();
        }
        catch (Exception ex)
        {
            app.Logger.LogError($"Failed running migrations. Err: {ex.Message}, Exception: {ex.InnerException}");
            await app.StopAsync();
        }

        app.Logger.LogInformation("Migration ran successfully");

        app.AddMiddlewares();
    }

    private static void RunDatabaseMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = (ApplicationDbContext)scope.ServiceProvider.GetRequiredService(typeof(ApplicationDbContext));
            db.Database.Migrate();
        }
    }

    private static void AddMiddlewares(this WebApplication app)
    {
        app.UseMiddleware(typeof(ErrorHandlingMiddleware));

        // Letting this here while I expose port 80 for testing certbot.
        //app.UseHsts();
        //app.UseHttpsRedirection();

        app.UseRouting();
        app.AddLetsEncryptChallengeEndpoint();
        app.UseRateLimiter();
        app.AddSwagger();
        app.UseAuthentication();
        app.UseAuthorization();
        app.EnableRequestBuffering();
        app.MapControllers();
    }

    private static void AddLetsEncryptChallengeEndpoint(this WebApplication app)
    {
        // Shared mounted volume where letsencrypt certbot will place the challenge files.
        app.UseStaticFiles(new StaticFileOptions
        {
            //FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "LetsEncrypt")),
            FileProvider = new PhysicalFileProvider("/letsencrypt"), // location of the folder in the docker container
            RequestPath = "/.well-known/acme-challenge"
        });
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
        //var sb = new StringBuilder();\
        FieldInfo applicationBuilderFieldInfo = app.GetType()
                            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                            .Single(pi => pi.Name == "<ApplicationBuilder>k__BackingField");

        object? applicationBuilderValue = applicationBuilderFieldInfo.GetValue(app);
        if (applicationBuilderValue != null)
        {
            Type appBuilderType = applicationBuilderValue.GetType();

            // Get the FieldInfo for the private field "_components" in ApplicationBuilder
            FieldInfo? _componentsField = appBuilderType.GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_componentsField != null)
            {
                List<Func<RequestDelegate, RequestDelegate>>? _componentsValue =
                    _componentsField.GetValue(applicationBuilderValue) as List<Func<RequestDelegate, RequestDelegate>>;

                if (_componentsValue != null)
                {
                    foreach (var func in _componentsValue)
                    {
                        var target = func.Target;

                        if (target != null)
                        {
                            Type targetType = target.GetType();
                            FieldInfo? stateFieldInfo = targetType.GetField("state");

                            if (stateFieldInfo != null)
                            {
                                var stateValue = stateFieldInfo.GetValue(target);

                                if (stateValue != null)
                                {
                                    Type stateType = stateValue.GetType();
                                    PropertyInfo middlewareFieldInfo = stateType.GetProperty("Middleware")!;
                                    object? middlewareValue = middlewareFieldInfo.GetValue(stateValue);

                                    if (middlewareValue != null)
                                    {
                                        Type middlewareType = middlewareValue.GetType();
                                        PropertyInfo nameInfo = middlewareType.GetProperty("Name")!;
                                        object nameValue = nameInfo.GetValue(middlewareValue)!;
                                        app.Logger.LogInformation($"Middleware: {nameValue}");
                                    }
                                }
                            }
                        }
                    }
                }
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
