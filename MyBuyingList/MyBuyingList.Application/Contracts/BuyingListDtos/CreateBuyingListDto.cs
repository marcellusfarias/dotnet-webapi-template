﻿using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.Common.Mappings;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Contracts.BuyingListDtos;

//Since I'm using DTO for incoming and outcoming data, I made some properties nullable
//so the user does not specify them on Creating a BuyingList
public class CreateBuyingListDto
{
    public required string Name { get; set; }
    public required int GroupId { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateBuyingListValidator : AbstractValidator<CreateBuyingListDto>
{
    public CreateBuyingListValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Please specify a name.");
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .WithMessage("Please specify a group.");
    }
}

[AutoMapperMapping]
public class CreateBuyingListDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<CreateBuyingListDto, BuyingList>();
    }
}