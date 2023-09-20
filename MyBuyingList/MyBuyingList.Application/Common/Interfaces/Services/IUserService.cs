using MyBuyingList.Application.DTOs;
using MyBuyingList.Application.DTOs.UserDtos;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IUserService
{
    GetUserDto GetUser(int id);
    Task<GetUserDto> GetUserAsync(int id);
    IEnumerable<GetUserDto> GetAllUsers();
    Task<IEnumerable<GetUserDto>> GetAllUsersAsync();
    
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns>New user Id.</returns>
    int Create(CreateUserDto userDto);

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
    void ChangeUserPassword(int userId, string oldPassword, string newPassword);
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
    void ChangeActiveStatus(int userId, bool activeStatus);
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
    void Delete(int userId);
    Task DeleteAsync(int userId);
}
