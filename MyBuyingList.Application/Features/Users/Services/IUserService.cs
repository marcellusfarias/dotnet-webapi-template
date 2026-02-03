using MyBuyingList.Application.Features.Users.DTOs;

namespace MyBuyingList.Application.Features.Users.Services;

/// <summary>
/// Provides user management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The user data.</returns>
    /// <exception cref="Application.Common.Exceptions.ResourceNotFoundException">Thrown when user is not found.</exception>
    Task<UserDto> GetUserAsync(int id, CancellationToken token);

    /// <summary>
    /// Retrieves a paginated list of all users.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A collection of user data.</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, CancellationToken token);

    /// <summary>
    /// Creates a new user with the provided data.
    /// </summary>
    /// <param name="userDto">The user creation data.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The identifier of the newly created user.</returns>
    Task<int> CreateAsync(CreateUserRequest userDto, CancellationToken token);

    /// <summary>
    /// Changes a user's password after validating the old password.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="oldPassword">The current password for verification.</param>
    /// <param name="newPassword">The new password to set.</param>
    /// <param name="token">Cancellation token.</param>
    /// <exception cref="Application.Common.Exceptions.ResourceNotFoundException">Thrown when user is not found.</exception>
    /// <exception cref="Application.Common.Exceptions.BusinessLogicException">Thrown when old password doesn't match.</exception>
    Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken token);

    /// <summary>
    /// Performs a soft delete by marking the user as inactive.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="token">Cancellation token.</param>
    /// <exception cref="Application.Common.Exceptions.ResourceNotFoundException">Thrown when user is not found.</exception>
    /// <exception cref="Application.Common.Exceptions.BusinessLogicException">Thrown when attempting to delete the admin user.</exception>
    Task DeleteAsync(int userId, CancellationToken token);
}
