using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MyBuyingList.Application.Common.CustomErrors;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Common;
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

    public IEnumerable<UserDto> List() => 
       _mapper.Map<IEnumerable<UserDto>>(_userRepository.GetAll());   
    
    public Result<bool, MyCustomError> Create(UserDto userDto)
    {
        ValidationResult validationResult = _validator.Validate(userDto);

        if(!validationResult.IsValid)
        {
            StringBuilder stringBuilder= new StringBuilder();
            validationResult.Errors.ForEach(err => stringBuilder.Append(err));

            return new MyCustomError(stringBuilder.ToString());
        }

        try
        {
            _userRepository.Add(_mapper.Map<User>(userDto));
        }
        catch (Exception ex)
        {
            return new MyCustomError(ex.Message);
        }
       
        return true;
    }
       
    
}
