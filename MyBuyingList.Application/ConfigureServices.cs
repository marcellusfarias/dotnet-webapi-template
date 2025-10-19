using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FluentValidation;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users.Services;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Services;

namespace MyBuyingList.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ILogger logger)
    {
        services.AddValidators();
        services.AddServices();

        return services;
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddTransient<IPasswordEncryptionService, PasswordEncryptionService>();
    }
}
