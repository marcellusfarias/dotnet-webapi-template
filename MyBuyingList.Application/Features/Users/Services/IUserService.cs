using MyBuyingList.Application.Features.Users.DTOs;

namespace MyBuyingList.Application.Features.Users.Services;

public interface IUserService
{
    Task<GetUserDto> GetUserAsync(int id, CancellationToken token);
    Task<IEnumerable<GetUserDto>> GetAllUsersAsync(int page, CancellationToken token);

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="userDto"></param>
    /// <param name="token"></param>
    /// <returns>New user Id.</returns>
    Task<int> CreateAsync(CreateUserDto userDto, CancellationToken token);

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="oldPassword"></param>
    /// <param name="newPassword"></param>
    /// <param name="token"></param>
    Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken token);

    /// <summary>
    /// Logical exclusion
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    Task DeleteAsync(int userId, CancellationToken token);
}
