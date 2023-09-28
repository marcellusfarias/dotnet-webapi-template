using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Infrastructure;
using MyBuyingList.Infrastructure.Repositories;
using MyBuyingList.Infrastructure.Auth.JwtSetup;
using MyBuyingList.Infrastructure.Auth.AuthorizationHandlers;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Infrastructure.Auth.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        string connectionString = GetConnectionString(configuration, logger);
        services.CreateDbContext(connectionString);
        services.AddRepositores();

        services.AddJwtAuthentication();
        services.AddAuthorizationConfiguration();        

        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddTransient<IJwtProvider, JwtProvider>();
    }

    private static void AddAuthorizationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void CreateDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                //.UseLazyLoadingProxies()
            );
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<ApplicationDbContext>();
    }

    private static void AddRepositores(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBuyingListRepository, BuyingListRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
    }

    private static string GetConnectionString(IConfiguration configuration, ILogger logger)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (connectionString == null)
        {
            logger.LogError($"Invalid connection string.");
            throw new Exception("Invalid connection string.");
        }

        return connectionString;
    }
}
