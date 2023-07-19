using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Services;

public class CustomAuthenticationService : ICustomAuthenticationService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserRepository _userRepository;

    public CustomAuthenticationService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public string AuthenticateAndReturnToken(string username, string password)
    {
        User? user = _userRepository.GetAuthenticationDataByUsername(username);
        if (user == null)
            throw new AuthenticationException(new Exception(), username);

        //TODO: unhash password

        if (password == user.Password)
            return _jwtProvider.Generate(user.Email);
        else
            throw new AuthenticationException(new Exception(), username);
    }
}
