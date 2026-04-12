using Microsoft.OpenApi.Models;
using MyBuyingList.Web.Filters;
using MyBuyingList.Application;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Web.Middlewares.Authorization;
using MyBuyingList.Web.Middlewares.CorrelationId;
using MyBuyingList.Web.Services;
using MyBuyingList.Web.Middlewares.RateLimiting;
using MyBuyingList.Application.Common.Options;
using MyBuyingList.Infrastructure;

namespace MyBuyingList.Web;

internal static class ConfigureServices
{
    internal static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExternalServices(configuration);

        services.AddRateLimitService(configuration);
        services.AddAuthorizationServices();
        services.AddSwaggerConfiguration();
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<CorrelationIdProvider>();
        services.AddScoped<ICorrelationIdProvider>(sp => sp.GetRequiredService<CorrelationIdProvider>());
        services.AddControllers(options => options.Filters.Add(typeof(RequestBodyValidationFilter)));
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddOptions<RefreshTokenOptions>()
            .BindConfiguration(RefreshTokenOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
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
                Description = "Enter JWT token with Bearer format: Bearer {token}"
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
        
        services.Configure<LockoutOptions>(configuration.GetSection(LockoutOptions.SectionName));
    }

    private static void AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices();
    }
}
