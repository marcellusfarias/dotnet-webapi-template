using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        var user = _userRepository.GetAuthenticationDataByUsername(username);
        if (user == null)
            throw new AuthenticationException(new Exception("Username does not exist"), username);

        //TODO: unhash password

        if (password == user.Password)
            return _jwtProvider.Generate(user.Email);
        else
            throw new AuthenticationException(new Exception("Wrong password"), username);
    }
}
