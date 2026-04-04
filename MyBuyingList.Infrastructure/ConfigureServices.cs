using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyBuyingList.Infrastructure;
using MyBuyingList.Infrastructure.Repositories;
using MyBuyingList.Infrastructure.Authentication.JwtSetup;
using MyBuyingList.Infrastructure.Persistence.Seeders;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Infrastructure.Authentication.Services;
using MyBuyingList.Application.Features.Users;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext(configuration);
        services.AddRepositories(configuration);
        services.AddJwtAuthentication();
        services.AddAdminSeeder();

        return services;
    }

    private static void AddAdminSeeder(this IServiceCollection services)
    {
        services.ConfigureOptions<AdminSettingsSetup>();
        services.AddScoped<AdminUserSeeder>();
    }

    private static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddTransient<IJwtProvider, JwtProvider>();
    }    

    private static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RepositorySettings>(configuration.GetSection("RepositorySettings"));
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = GetConnectionString(configuration);

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

    private static string GetConnectionString(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (connectionString == null)
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        return connectionString;
    }
}
