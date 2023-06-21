using MyBuyingList.Application.DTOs;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IUserService
{
    IEnumerable<UserDto> List();
    void Create(UserDto userDto);
}
