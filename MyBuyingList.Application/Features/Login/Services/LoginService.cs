using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace MyBuyingList.Application.Features.Login.Services;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly LockoutOptions _lockoutOptions;

    public LoginService(
        IUserRepository userRepository,
        IJwtProvider jwtProvider, 
        IPasswordEncryptionService passwordEncryptionService, 
        IOptions<LockoutOptions> lockoutOptions)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _passwordEncryptionService = passwordEncryptionService;
        _lockoutOptions = lockoutOptions.Value;
    }

    public async Task<string> AuthenticateAndReturnJwtTokenAsync(LoginRequest loginDto, CancellationToken token)
    {
        var username = loginDto.Username.ToLower();
        var password = loginDto.Password;

        User? user = await _userRepository.GetActiveUserByUsernameAsync(username, token);
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

            await _userRepository.IncrementFailedLoginAttemptsAsync(user.Id, lockoutEnd, token);

            if (lockoutEnd != null)
            {
                throw new AccountLockedException(username, _lockoutOptions.LockoutDurationMinutes);
            }

            throw new AuthenticationException(username,
                $"{ErrorMessages.InvalidUsernameOrPassword} Attempt {newCount} of {_lockoutOptions.MaxFailedAttempts}.");
        }

        if (user.FailedLoginAttempts > 0)
        {
            await _userRepository.ResetLockoutAsync(user.Id, token);
        }
        
        return await _jwtProvider.GenerateTokenAsync(user.Id, token);
    }
}
