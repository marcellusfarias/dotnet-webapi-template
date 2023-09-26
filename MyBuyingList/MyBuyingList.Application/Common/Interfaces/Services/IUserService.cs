using MyBuyingList.Application.DTOs;
using MyBuyingList.Application.DTOs.UserDtos;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IUserService
{
    Task<GetUserDto> GetUserAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new user asyncronously.
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns>New user Id.</returns>
    Task<int> CreateAsync(CreateUserDto userDto);

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="oldPassword"></param>
    /// <param name="newPassword"></param>
    Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activeStatus"></param>
    Task ChangeActiveStatusAsync(int userId, bool activeStatus);
    /// <summary>
    /// Logical exclusion
    /// </summary>
    /// <param name="userId"></param>
    Task DeleteAsync(int userId);
}
