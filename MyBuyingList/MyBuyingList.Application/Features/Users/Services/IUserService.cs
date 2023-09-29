using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users.Services;

public interface IUserService
{
    Task<GetUserDto> GetUserAsync(int id, CancellationToken token);
    Task<IEnumerable<GetUserDto>> GetAllUsersAsync(CancellationToken token);

    /// <summary>
    /// Creates a new user asyncronously.
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns>New user Id.</returns>
    Task<int> CreateAsync(CreateUserDto userDto, CancellationToken token);

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="oldPassword"></param>
    /// <param name="newPassword"></param>
    Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken token);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activeStatus"></param>
    Task ChangeActiveStatusAsync(int userId, bool activeStatus, CancellationToken token);
    /// <summary>
    /// Logical exclusion
    /// </summary>
    /// <param name="userId"></param>
    Task DeleteAsync(int userId, CancellationToken token);
}
