using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyBuyingList.Infrastructure;
using MyBuyingList.Infrastructure.Repositories;
using MyBuyingList.Infrastructure.Authentication.JwtSetup;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Infrastructure.Authentication.Services;
using MyBuyingList.Application.Features.Users;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        services.AddDatabaseContext(configuration, logger);
        services.AddRepositores(configuration);
        services.AddJwtAuthentication();
        
        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddTransient<IJwtProvider, JwtProvider>();
    }    

    private static void AddRepositores(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RepositorySettings>(configuration.GetSection("RepositorySettings"));
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        string connectionString = GetConnectionString(configuration, logger);

        services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options
                        .UseNpgsql(connectionString)
                        .UseSnakeCaseNamingConvention();
                }
            );
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<ApplicationDbContext>();
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
