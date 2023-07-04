using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Services;
using System.Reflection;
using AutoMapper;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;
using FluentValidation;

namespace MyBuyingList.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ILogger logger)
    {
        logger.LogInformation("Adding application services");
        services.AddScoped<IUserService, UserService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>();
            cfg.CreateMap<UserDto, User>();
        });
        
        // use DI (http://docs.automapper.org/en/latest/Dependency-injection.html) or create the mapper yourself
        IMapper mapper = configuration.CreateMapper();
        services.AddSingleton(mapper);

        services.AddScoped<IValidator<UserDto>, UserValidator>();
        return services;
    }
}
