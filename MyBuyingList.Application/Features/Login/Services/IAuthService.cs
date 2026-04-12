using MyBuyingList.Application.Features.Login.DTOs;

namespace MyBuyingList.Application.Features.Login.Services;

/// <summary>
/// Provides authentication functionality, including login and token refresh.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and returns an access token and a refresh token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="LoginResponse"/> containing both tokens and their expiration timestamps.</returns>
    /// <exception cref="MyBuyingList.Application.Common.Exceptions.AuthenticationException">Thrown when credentials are invalid.</exception>
    Task<LoginResponse> AuthenticateAsync(LoginRequest loginDto, CancellationToken cancellationToken);

    /// <summary>
    /// Exchanges a valid refresh token for a new access token and a new refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="LoginResponse"/> containing both new tokens and their expiration timestamps.</returns>
    /// <exception cref="MyBuyingList.Application.Common.Exceptions.AuthenticationException">Thrown when the refresh token is invalid, expired, or revoked.</exception>
    Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken);
}
