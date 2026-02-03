using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Mappers;

namespace MyBuyingList.Application.Features.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    public UserService(IUserRepository userRepository, IPasswordEncryptionService passwordEncryptionService)
    {
        _userRepository = userRepository;
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task<UserDto> GetUserAsync(int userId, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);

        if (user is null)
            throw new ResourceNotFoundException();

        return user.ToUserDto();
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(int page, CancellationToken token)
    {
        var users = await _userRepository.GetAllAsync(page, token);
        List<UserDto> getUserDtos = [];
        users.ForEach(user => getUserDtos.Add(user.ToUserDto()));
        return getUserDtos;
    }

    public async Task<int> CreateAsync(CreateUserRequest userDto, CancellationToken token)
    {
        var user = userDto.ToUser(active: true);
        user.Password = _passwordEncryptionService.HashPassword(userDto.Password);

        return await _userRepository.AddAsync(user, token);
    }

    public async Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);
        if (user is null)
            throw new ResourceNotFoundException();

        bool checkOldPassword = _passwordEncryptionService.VerifyPassword(oldPassword, user.Password);
        if (!checkOldPassword)
            throw new BusinessLogicException("Old password does not match current one.");

        user.Password = _passwordEncryptionService.HashPassword(newPassword);

        await _userRepository.EditAsync(user, token);
    }

    public async Task DeleteAsync(int userId, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);

        if (user is null)
            throw new ResourceNotFoundException();

        if (user.UserName.Equals(Domain.Constants.Users.AdminUser.UserName, StringComparison.OrdinalIgnoreCase))
            throw new BusinessLogicException("Can't disable user admin.");

        await _userRepository.DeactivateAsync(user, token);
    }
}
