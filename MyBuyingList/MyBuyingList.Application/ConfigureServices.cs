using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FluentValidation;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users.Services;
using MyBuyingList.Application.Features.BuyingLists.Services;
using MyBuyingList.Application.Features.Groups.Services;

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
        //services.AddScoped<IValidator<UserDto>, UserValidator>();
        //services.AddScoped<IValidator<BuyingListDto>, BuyingListValidator>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IBuyingListService, BuyingListService>();
        services.AddScoped<IGroupService, GroupService>();
    }
}
