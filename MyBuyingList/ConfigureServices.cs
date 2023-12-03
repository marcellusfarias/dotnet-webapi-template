using Microsoft.OpenApi.Models;
using MyBuyingList.Web.Filters;
using MyBuyingList.Application;
using Microsoft.AspNetCore.MiddlewareAnalysis;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MyBuyingList.Web.Middlewares.Authorization;
using MyBuyingList.Web.Middlewares.RateLimiting;

namespace MyBuyingList.Web;

internal static class ConfigureServices
{
    internal static void AddServices(this IServiceCollection services, ILogger logger, IConfiguration configuration, bool isDevelopment)
    {
        // Insert the AnalysisStartupFilter as the first IStartupFilter in the container
        //if (isDevelopment) services.Insert(0, ServiceDescriptor.Transient<IStartupFilter, AnalysisStartupFilter>());

        services.AddExternalServices(logger, configuration);

        // Rate limiter added just for the authentication endpoint
        services.Configure<CustomRateLimiterOptions>(configuration.GetSection("CustomRateLimiterOptions"));
        services.AddRateLimiter(options =>
        {
            options.AddPolicy<IPAddress, AuthenticationRateLimiterPolicy>("Authentication");
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddControllers(options => { options.Filters.Add(typeof(RequestBodyValidationFilter)); });
        services.AddLogging();

        if (isDevelopment)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MyBuyingList API",
                    Version = "v1",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Here enter JWT Token with bearer format like bearer[space] token"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }
    }

    // Add services from other projects.
    private static void AddExternalServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        services.AddInfrastructureServices(logger, configuration);
        services.AddApplicationServices(logger);
    }

    // will use this when changing this from API to APP
    private static void AddServicesForWebApp(this IServiceCollection services)
    {
        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
            options.HttpsPort = 5001;
        });

        //builder.Services.AddHttpsRedirection(options =>
        //{
        //    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        //    options.HttpsPort = 443;
        //});
    }
}
