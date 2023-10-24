using Microsoft.EntityFrameworkCore;
using MyBuyingList.Infrastructure;
using MyBuyingList.Web.Middlewares;
using System.Diagnostics;
using System.Reflection;

namespace MyBuyingList.Web;

internal static class ConfigureApp
{
    internal async static Task<WebApplication> StartApplication(this WebApplication app)
    {
        bool isDevelopment = app.Environment.IsDevelopment();
        
        try
        {
            app.RunDatabaseMigrations();
        }
        catch (Exception ex)
        {
            app.Logger.LogError($"Failed running migrations. Err: {ex.Message}, Exception: {ex.InnerException}");
            await app.StopAsync();
        }

        //if (isDevelopment)
        //    app.AddDebuggingMiddlewareLogic();

        app.AddMiddlewares(isDevelopment);

        return app;
    }

    private static void RunDatabaseMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = (ApplicationDbContext)scope.ServiceProvider.GetRequiredService(typeof(ApplicationDbContext));
            db.Database.Migrate();
        }
    }

    private static void AddMiddlewares(this WebApplication app, bool isDevelopment)
    {
        app.UseMiddleware(typeof(ErrorHandlingMiddleware));
        app.UseRouting();
        app.UseRateLimiter();

        if (isDevelopment)
        {
            // Don't require authentication for this.
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();              

        // Needed for RequestBodyValidationFilter, so it can access the request body more than one time for doing the validation.
        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });

        app.MapControllers();
    }

    #region UsefulFunctions

    //Not needed for now, as this project is still only an API.
    private static void ApplyWebConfigs(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

            //builder.Services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(60);
            //    options.ExcludedHosts.Add("example.com");
            //    options.ExcludedHosts.Add("www.example.com");
            //});
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
    }

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
