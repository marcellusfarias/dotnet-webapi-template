using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users.Mappers;

public static class UserMapperDomainToDto
{
    public static GetUserDto ToGetUserDto(this User user)
    {
        return new GetUserDto(user.Id, user.UserName, user.Email, user.Active);
    }
}
