﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Services;
using System.Reflection;
using AutoMapper;
using MyBuyingList.Application.DTOs;
using FluentValidation;
using MyBuyingList.Application.Common.Mappings;

namespace MyBuyingList.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ILogger logger)
    {
        services.AddAutoMapperConfiguration();
        services.AddValidators();
        services.AddServices();

        return services;
    }

    private static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        // use DI (http://docs.automapper.org/en/latest/Dependency-injection.html) or create the mapper yourself
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DefaultProfile());
        });

        services.AddSingleton(configuration.CreateMapper());
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<UserDto>, UserValidator>();
        services.AddScoped<IValidator<BuyingListDto>, BuyingListValidator>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IBuyingListService, BuyingListService>();
    }
}
