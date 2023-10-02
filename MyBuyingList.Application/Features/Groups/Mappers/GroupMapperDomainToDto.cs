using MyBuyingList.Application.Features.Groups.DTOs;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Features.Groups.Mappers;

public static class GroupMapperDomainToDto
{
    public static GetGroupDto ToGetGroupDto(this Group group)
    {
        return new GetGroupDto
        {
            Id = group.Id,
            GroupName = group.GroupName,
        };
    }
}
