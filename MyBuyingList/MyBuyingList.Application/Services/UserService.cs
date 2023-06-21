using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;
using System.Text;

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
        IEnumerable<UserDto> list = _mapper.Map<IEnumerable<UserDto>>(users); //maybe get thi exception?
        return list;
    }

    public void Create(UserDto userDto)
    {
        ValidationResult validationResult = _validator.Validate(userDto);

        if (!validationResult.IsValid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            validationResult.Errors.ForEach(err => stringBuilder.Append(err));

            throw new ValidationException(stringBuilder.ToString());
        }

        _userRepository.Add(_mapper.Map<User>(userDto));
    }
}
