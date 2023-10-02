using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users.Mappers;

public static class UserMapperDomainToDto
{
    public static GetUserDto ToGetUserDto(this User user)
    {
        return new GetUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Active = user.Active,
        };
    }
}
