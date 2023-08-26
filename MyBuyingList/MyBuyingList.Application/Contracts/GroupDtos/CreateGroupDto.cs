using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.Common.Mappings;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Contracts.GroupDtos;

public class CreateGroupDto
{
    public required string GroupName { get; set; }
}

public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
{
    public CreateGroupDtoValidator()
    {
        RuleFor(x => x.GroupName).NotEmpty().MaximumLength(256);
    }
}

[AutoMapperMapping]
public class CreateGroupDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<CreateGroupDto, Group>();
    }
}