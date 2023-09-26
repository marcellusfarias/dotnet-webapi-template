using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Application.Services;

public class LoginService : ILoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    public LoginService(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> AuthenticateAndReturnJwtTokenAsync(string username, string password, CancellationToken token)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            throw new AuthenticationException(new Exception("Empty username or password"), username);

        //should I create a repository method?
        var users = await _userRepository.GetActiveUsersAsync(token);
        User? user = users.Where(x => x.UserName == username).FirstOrDefault();
        if (user == null)
            throw new AuthenticationException(new Exception("Invalid credentials"), username);
        
        var unhashedPassword = user.Password; //TODO !
        if (password != unhashedPassword)
            throw new AuthenticationException(new Exception("Invalid credentials"), username);

        return await _jwtProvider.GenerateTokenAsync(user.Id, token);
    }
}
