using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.DTOs.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Contracts.BuyingListDtos;

public class GetBuyingListDto
{
}

public class GetBuyingListDtoValidator : AbstractValidator<GetBuyingListDto>
{
    public GetBuyingListDtoValidator()
    {
        
    }
}

[AutoMapperMapping]
public class CreateUserDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<GetBuyingListDto, BuyingList>();
    }
}