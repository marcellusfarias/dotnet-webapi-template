using MyBuyingList.Application.Features.Login.DTOs;

namespace MyBuyingList.Application.Features.Login.Services;

/// <summary>
/// Provides user authentication functionality.
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT token if authentication succeeds.</returns>
    /// <exception cref="MyBuyingList.Application.Common.Exceptions.AuthenticationException">Thrown when credentials are invalid.</exception>
    Task<string> AuthenticateAndReturnJwtTokenAsync(LoginRequest loginDto, CancellationToken cancellationToken);
}
