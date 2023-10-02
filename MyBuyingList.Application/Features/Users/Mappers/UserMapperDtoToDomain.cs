using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users.Mappers;

public static class UserMapperDtoToDomain
{
    public static User ToUser(this CreateUserDto dto, bool active)
    {
        return new User
        {
            Id = 0,
            UserName = dto.UserName,
            Password = dto.Password,
            Email = dto.Email,
            Active = active,
        };
    }
}
