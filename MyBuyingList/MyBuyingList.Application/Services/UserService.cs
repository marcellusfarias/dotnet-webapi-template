using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.Common.Extensions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<UserDto> _validator;

    public UserService(IUserRepository userRepository, IMapper mapper, IValidator<UserDto> validator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public IEnumerable<UserDto> List()
    {
        IEnumerable<User> users = _userRepository.GetAll();
        IEnumerable<UserDto> list = _mapper.Map<IEnumerable<UserDto>>(users); //maybe get this exception?
        return list;
    }

    public void Create(UserDto userDto)
    {
        _validator.ValidateAndThrowCustomException(userDto);
        _userRepository.Add(_mapper.Map<User>(userDto));
    }

    public void Update(UserDto userDto)
    {
        _validator.ValidateAndThrowCustomException(userDto);
        _userRepository.Edit(_mapper.Map<User>(userDto));
    }

    public void Delete(UserDto userDto)
    {
        _validator.ValidateAndThrowCustomException(userDto);
        _userRepository.Delete(_mapper.Map<User>(userDto));
    }
}
