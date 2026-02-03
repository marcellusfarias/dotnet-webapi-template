
namespace MyBuyingList.Application.Common.Interfaces;

/// <summary>
/// Provides JWT token generation functionality.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT token string.</returns>
    Task<string> GenerateTokenAsync(int userId, CancellationToken cancellationToken);
}
