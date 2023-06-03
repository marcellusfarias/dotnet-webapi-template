using MyBuyingList.Application.Common.CustomErrors;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Domain.Common;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IUserService
{
    //TODO: add mapping to ViewModel or DTO
    IEnumerable<UserDto> List();
    Result<bool, MyCustomError> Create(UserDto userDto);
}
