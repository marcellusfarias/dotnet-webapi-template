using AutoMapper;
using MyBuyingList.Application.Common.Mappings;
using MyBuyingList.Application.Contracts.BuyingListDtos;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Contracts.GroupDtos;

public class GetGroupDto
{
    public required int Id { get; set; }
    public required string GroupName { get; set; }
}

[AutoMapperMapping]
public class GetGroupDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Group, GetGroupDto>();
    }
}