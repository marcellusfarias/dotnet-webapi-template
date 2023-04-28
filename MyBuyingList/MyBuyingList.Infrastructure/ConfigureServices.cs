using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyBuyingList.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Npgsql;
using Microsoft.Extensions.Logging;
using MyBuyingList.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection"); //log and throw error
        logger.LogInformation($"Connection string: {connectionString}");

        services
            .AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
            );

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
