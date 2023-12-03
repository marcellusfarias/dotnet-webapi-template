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

    public async Task<GetUserDto> GetUserAsync(int userId, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);

        if (user is null)
            throw new ResourceNotFoundException();

        return user.ToGetUserDto();
    }

    public async Task<IEnumerable<GetUserDto>> GetAllUsersAsync(CancellationToken token)
    {
        var users = await _userRepository.GetAllAsync(token);
        List<GetUserDto> getUserDtos = new List<GetUserDto>();
        users.ForEach(user => getUserDtos.Add(user.ToGetUserDto()));
        return getUserDtos;
    }

    public async Task<int> CreateAsync(CreateUserDto userDto, CancellationToken token)
    {
        var user = userDto.ToUser(active: true);
        user.Password = _passwordEncryptionService.HashPassword(userDto.Password); ;

        return await _userRepository.AddAsync(user, token);
    }

    public async Task ChangeActiveStatusAsync(int userId, bool activeStatus, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);
        if (user is null)
            throw new ResourceNotFoundException();

        user.Active = activeStatus;

        await _userRepository.EditAsync(user, token);
    }

    public async Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);
        if (user is null)
            throw new ResourceNotFoundException();

        bool checkOldPassword = _passwordEncryptionService.VerifyPasswordsAreEqual(oldPassword, user.Password);
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

        if (user.UserName.Equals("admin"))
            throw new BusinessLogicException("Can't disable user admin.");

        await _userRepository.LogicalExclusionAsync(user, token);
    }
}
