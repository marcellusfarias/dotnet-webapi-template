using AutoMapper;
using MyBuyingList.Application.Common.Mappings;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.DTOs.UserDtos;

public class GetUserDto
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required bool Active { get; set; }
}

[AutoMapperMapping]
public class GetUserDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
        => cfg.CreateMap<User, GetUserDto>();    
}