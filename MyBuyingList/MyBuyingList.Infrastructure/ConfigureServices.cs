using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyBuyingList.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Npgsql;
using Microsoft.Extensions.Logging;
using MyBuyingList.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Infrastructure.Repositories;
using MyBuyingList.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyBuyingList.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        string connectionString = GetConnectionString(configuration, logger);        
        services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
            );
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<ApplicationDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddJwtAuthentication();

        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddTransient<IJwtProvider, JwtProvider>();
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();        
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
