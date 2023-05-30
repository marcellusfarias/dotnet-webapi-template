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

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ILogger logger, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection"); //log and throw error
        services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
            );
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddScoped<ApplicationDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}
