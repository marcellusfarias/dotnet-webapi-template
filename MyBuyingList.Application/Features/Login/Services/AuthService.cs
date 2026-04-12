using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Options;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Features.Login.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly LockoutOptions _lockoutOptions;
    private readonly RefreshTokenOptions _refreshTokenOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IJwtProvider jwtProvider,
        IPasswordEncryptionService passwordEncryptionService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<LockoutOptions> lockoutOptions,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _passwordEncryptionService = passwordEncryptionService;
        _refreshTokenRepository = refreshTokenRepository;
        _lockoutOptions = lockoutOptions.Value;
        _refreshTokenOptions = refreshTokenOptions.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> AuthenticateAsync(LoginRequest loginDto, CancellationToken cancellationToken)
    {
        var username = loginDto.Username.ToLower();
        var password = loginDto.Password;

        User? user = await _userRepository.GetActiveUserByUsernameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new AuthenticationException(username, ErrorMessages.InvalidUsernameOrPassword);
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
        {
            var minutesRemaining = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            throw new AccountLockedException(username, minutesRemaining);
        }

        bool verification = _passwordEncryptionService.VerifyPassword(password, user.Password);
        if (!verification)
        {
            var newCount = user.FailedLoginAttempts + 1;
            DateTime? lockoutEnd = newCount >= _lockoutOptions.MaxFailedAttempts
                ? DateTime.UtcNow.AddMinutes(_lockoutOptions.LockoutDurationMinutes)
                : null;

            await _userRepository.IncrementFailedLoginAttemptsAsync(user.Id, lockoutEnd, cancellationToken);

            if (lockoutEnd != null)
            {
                throw new AccountLockedException(username, _lockoutOptions.LockoutDurationMinutes);
            }

            throw new AuthenticationException(username,
                $"{ErrorMessages.InvalidUsernameOrPassword} Attempt {newCount} of {_lockoutOptions.MaxFailedAttempts}.");
        }

        if (user.FailedLoginAttempts > 0)
        {
            await _userRepository.ResetLockoutAsync(user.Id, cancellationToken);
        }

        var (accessToken, accessTokenExpiresAt) = await _jwtProvider.GenerateTokenAsync(user.Id, cancellationToken);
        var (rawRefreshToken, refreshTokenEntity) = CreateRefreshToken(user.Id, _refreshTokenOptions.ExpirationDays);
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        return new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt
        };
    }

    public async Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
    {
        var hash = _passwordEncryptionService.ComputeRefreshTokenHash(request.RefreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(hash, cancellationToken)
            ?? throw new AuthenticationException("Invalid refresh token.");
        
        if (storedToken.IsRevoked)
        {
            // A revoked token has been reused — this signals likely token theft.
            // We intentionally revoke ALL tokens for this user (nuclear revocation):
            // all active sessions across all devices are invalidated.
            // This is a deliberate trade-off: other legitimate devices will be logged out,
            // but this is the safest response to a potentially compromised token. See BACKLOG.md for the
            // token-families alternative that would allow per-session revocation.
            _logger.LogWarning(
                "Suspicious activity: revoked refresh token reused for user {UserId}. Revoking all tokens.",
                storedToken.UserId);

            await _refreshTokenRepository.RevokeAllForUserAsync(storedToken.UserId, cancellationToken);
            throw new AuthenticationException(storedToken.User.UserName, "Invalid or expired refresh token.");
        }
        
        if (storedToken.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            throw new AuthenticationException("Expired refresh token.");
        }

        var (accessToken, accessTokenExpiresAt) = await _jwtProvider.GenerateTokenAsync(storedToken.UserId, cancellationToken);
        var (rawToken, newTokenEntity) = CreateRefreshToken(storedToken.UserId, _refreshTokenOptions.ExpirationDays);

        await _refreshTokenRepository.RevokeAndAddAsync(storedToken, newTokenEntity, cancellationToken);
        
        return new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = rawToken,
            RefreshTokenExpiresAt = newTokenEntity.ExpiresAt
        };
    }

    private (string RawToken, RefreshToken Entity) CreateRefreshToken(int userId, int expirationDays)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays);

        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = _passwordEncryptionService.ComputeRefreshTokenHash(rawToken),
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };

        return (rawToken, entity);
    }
}
