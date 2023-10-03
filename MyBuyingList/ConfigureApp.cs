using Microsoft.EntityFrameworkCore;
using MyBuyingList.Infrastructure;
using MyBuyingList.Web.Middlewares;
using System.Diagnostics;

namespace MyBuyingList.Web;

internal static class ConfigureApp
{
    internal static WebApplication StartApplication(this WebApplication app, ILogger<Program> logger)
    {
        app.AddDebuggingMiddlewareLogic();
        app.EnableRequestBuffering();
        app.StartSwagger();
        app.RunMigrations(logger);
        app.AddMiddlewares();

        return app;
    }

    private static void AddDebuggingMiddlewareLogic(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Grab the "Microsoft.AspNetCore" DiagnosticListener from DI
            var listener = app.Services.GetRequiredService<DiagnosticListener>();

            // Create an instance of the AnalysisDiagnosticAdapter using the IServiceProvider
            // so that the ILogger is injected from DI
            var observer = ActivatorUtilities.CreateInstance<AnalysisDiagnosticAdapter>(app.Services);

            // Subscribe to the listener with the SubscribeWithAdapter() extension method
            using var disposable = listener.SubscribeWithAdapter(observer);
        }
    }

    // Needed for RequestBodyValidationFilter, so it can access the request body more than one time for doing the validation.
    private static void EnableRequestBuffering(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            context.Request.EnableBuffering();
            return next();
        });
    }

    private static void StartSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }

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

    private static void RunMigrations(this WebApplication app, ILogger<Program> logger)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = (ApplicationDbContext)scope.ServiceProvider.GetRequiredService(typeof(ApplicationDbContext));
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed running migrations. Err: {ex.Message}, Exception: {ex.InnerException}");
            }
        }
    }

    private static void AddMiddlewares(this WebApplication app)
    {
        app.UseRouting();

        app.UseMiddleware(typeof(ErrorHandlingMiddleware));
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}
