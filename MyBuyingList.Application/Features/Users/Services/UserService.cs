using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Mappers;

namespace MyBuyingList.Application.Features.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

    //TODO: perform validations. need to hash password before storing into DB.
    public async Task<int> CreateAsync(CreateUserDto userDto, CancellationToken token)
    {
        var user = userDto.ToUser(true);
        
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

    //TODO: perform validations. need to do hashing to compare passwords.
    public async Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);
        if (user is null)
            throw new ResourceNotFoundException();

        if (user.Password != oldPassword)
            throw new BusinessLogicException("Old password does not match current one.");

        user.Password = newPassword;

        await _userRepository.EditAsync(user, token);
    }

    // must perform validations
    public async Task DeleteAsync(int userId, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);

        if (user is null)
            throw new ResourceNotFoundException();

        await _userRepository.LogicalExclusionAsync(user, token);
    }
}
