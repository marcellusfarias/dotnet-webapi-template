using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users.Mappers;

public static class UserMapperDomainToDto
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto(user.Id, user.UserName, user.Email, user.Active);
    }
}
