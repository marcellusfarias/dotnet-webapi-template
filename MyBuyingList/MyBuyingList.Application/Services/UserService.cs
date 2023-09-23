using AutoMapper;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.DTOs.UserDtos;
using Microsoft.VisualBasic;

namespace MyBuyingList.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public GetUserDto GetUser(int userId)
    {
        var user = _userRepository.Get(userId);
        return user == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<GetUserDto>(user);
    }
    public async Task<GetUserDto> GetUserAsync(int userId)
    {
        var user = await _userRepository.GetAsync(userId);
        return user == null
            ? throw new ResourceNotFoundException()
            : _mapper.Map<GetUserDto>(user);
    }

    public IEnumerable<GetUserDto> GetAllUsers()
    {
        IEnumerable<User> users = _userRepository.GetAll();
        IEnumerable<GetUserDto> list = _mapper.Map<IEnumerable<GetUserDto>>(users); //map exceptions?
        return list;
    }
    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _userRepository.GetAllAsync(cancellationToken);
        //IEnumerable<GetUserDto> list = _mapper.Map<IEnumerable<GetUserDto>>(users); //map exceptions?
        //return list;

        var returningList = new List<User>();
        users.ToList().ForEach(u => returningList.Add(new User
        {
            Active = u.Active,
            Email = u.Email,
            Password = u.Password,
            UserName = u.UserName,
            BuyingListCreatedBy = new List<BuyingList>(),
            CreatedAt = u.CreatedAt,
            GroupsCreatedBy = new List<Group>(),
            Id = u.Id,
            UserRoles = new List<UserRole>()
        }));;

        return returningList;
    }

    public int Create(CreateUserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        user.Active = true;
        // hash password, check if password is ok

        return _userRepository.Add(user);
    }

    public async Task<int> CreateAsync(CreateUserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        user.Active = true;
        // hash password, check if password is ok

        return await _userRepository.AddAsync(user);
    }

    //test NULL active status
    public void ChangeActiveStatus(int userId, bool activeStatus)
    {
        var user = _userRepository.Get(userId);
        if (user is null)
            throw new ResourceNotFoundException();

        user.Active = activeStatus;

        _userRepository.Edit(user);
    }

    //test NULL active status
    public async Task ChangeActiveStatusAsync(int userId, bool activeStatus)
    {
        var user = await _userRepository.GetAsync(userId);
        if (user is null)
            throw new ResourceNotFoundException();

        user.Active = activeStatus;

        await _userRepository.EditAsync(user);
    }

    public void ChangeUserPassword(int userId, string oldPassword, string newPassword)
    {
        var user = _userRepository.Get(userId);
        if (user is null)
            throw new ResourceNotFoundException();

        if (user.Password != oldPassword)
            throw new BusinessLogicException("Old password does not match current one.");

        user.Password = newPassword;

        _userRepository.Edit(user);
    }

    public async Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetAsync(userId);
        if (user is null)
            throw new ResourceNotFoundException();

        if (user.Password != oldPassword)
            throw new BusinessLogicException("Old password does not match current one.");

        user.Password = newPassword;

        await _userRepository.EditAsync(user);
    }

    public void Delete(int userId)
    {
        var user = _userRepository.Get(userId);

        if (user == null)
            throw new ResourceNotFoundException();

        _userRepository.LogicalExclusion(user);
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await _userRepository.GetAsync(userId);

        if (user == null)
            throw new ResourceNotFoundException();

        await _userRepository.LogicalExclusionAsync(user);
    }
}
