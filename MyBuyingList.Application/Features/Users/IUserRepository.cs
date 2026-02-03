using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Users;

/// <summary>
/// Repository interface for user-specific data operations.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves an active user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The user if found and active; otherwise, null.</returns>
    Task<User?> GetActiveUserByUsernameAsync(string username, CancellationToken token);

    /// <summary>
    /// Retrieves all policies assigned to a user through their roles.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A list of policies assigned to the user; null if user not found.</returns>
    Task<List<Policy>?> GetUserPoliciesAsync(int id, CancellationToken token);

    /// <summary>
    /// Performs a soft delete by marking the user as inactive.
    /// </summary>
    /// <param name="user">The user to deactivate.</param>
    /// <param name="token">Cancellation token.</param>
    Task LogicalExclusionAsync(User user, CancellationToken token);
}
