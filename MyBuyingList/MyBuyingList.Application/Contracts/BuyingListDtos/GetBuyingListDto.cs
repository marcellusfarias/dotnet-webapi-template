using AutoMapper;
using FluentValidation;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Mappings;

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