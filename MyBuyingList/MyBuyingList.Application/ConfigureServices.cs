using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Execution;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Interfaces.Repositories;

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
        });
        
        // use DI (http://docs.automapper.org/en/latest/Dependency-injection.html) or create the mapper yourself
        var mapper = configuration.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
}
