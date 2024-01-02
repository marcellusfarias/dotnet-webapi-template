using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Common.Constants;

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

    public async Task<string> AuthenticateAndReturnJwtTokenAsync(LoginDto loginDto, CancellationToken token)
    {
        var username = loginDto.Username.ToLower();
        var password = loginDto.Password;
     
        User? user = await _userRepository.GetActiveUserByUsername(username, token);
        if (user is null)
            throw new AuthenticationException(username, ErrorMessages.InvalidUsernameOrPassword);

        bool verification = _passwordEncryptionService.VerifyPasswordsAreEqual(password, user.Password);
        if (!verification)
            throw new AuthenticationException(username, ErrorMessages.InvalidUsernameOrPassword);

        return await _jwtProvider.GenerateTokenAsync(user.Id, token);
    }
}
