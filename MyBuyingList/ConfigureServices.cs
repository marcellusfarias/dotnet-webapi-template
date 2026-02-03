using Microsoft.OpenApi.Models;
using MyBuyingList.Web.Filters;
using MyBuyingList.Application;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MyBuyingList.Web.Middlewares.Authorization;
using MyBuyingList.Web.Middlewares.RateLimiting;

namespace MyBuyingList.Web;

internal static class ConfigureServices
{
    internal static void AddServices(this IServiceCollection services, ILogger logger, IConfiguration configuration, bool isDevelopment)
    {
        services.AddExternalServices(logger, configuration);

        services.AddRateLimitService(configuration);
        services.AddAuthorizationServices();
        services.AddSwaggerConfiguration();
        services.AddControllers(options => options.Filters.Add(typeof(RequestBodyValidationFilter)));
        services.AddLogging();
    }

    private static void AddSwaggerConfiguration(this IServiceCollection services)
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
                        []
                    }
                });
        });
    }

    private static void AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void AddRateLimitService(this IServiceCollection services, IConfiguration configuration)
    {
        // Rate limiter added just for the authentication endpoint
        services.Configure<CustomRateLimiterOptions>(configuration.GetSection("CustomRateLimiterOptions"));
        services.AddRateLimiter(options =>
        {
            options.AddPolicy<IPAddress, AuthenticationRateLimiterPolicy>(AuthenticationRateLimiterPolicy.PolicyName);
        });
    }

    private static void AddExternalServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        services.AddInfrastructureServices(logger, configuration);
        services.AddApplicationServices();
    }
}
