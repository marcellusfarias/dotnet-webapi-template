using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Users;

namespace MyBuyingList.Application.Features.Login.Services;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordEncryptionService _passwordEncryptionService;

    public LoginService(IUserRepository userRepository, IJwtProvider jwtProvider, IPasswordEncryptionService passwordEncryptionService)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task<string> AuthenticateAndReturnJwtTokenAsync(string username, string password, CancellationToken token)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new AuthenticationException(username, "Empty username or password.");

        User? user = await _userRepository.GetActiveUserByUsername(username, token);
        if (user is null)
            throw new AuthenticationException(username, "Invalid username or password.");

        bool verification = _passwordEncryptionService.VerifyPasswordsAreEqual(password, user.Password);
        if (!verification)
            throw new AuthenticationException(username, "Invalid username or password.");

        return await _jwtProvider.GenerateTokenAsync(user.Id, token);
    }
}
