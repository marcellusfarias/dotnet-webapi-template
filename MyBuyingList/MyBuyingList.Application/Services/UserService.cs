using AutoMapper;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    //TODO: add mapping to ViewModel or DTO
    public IEnumerable<UserDto> List() => 
       _mapper.Map<IEnumerable<UserDto>>(_userRepository.GetAll());   
    
    public void Create(UserDto userDto) =>
       _userRepository.Add(_mapper.Map<User>(userDto));
    
}
