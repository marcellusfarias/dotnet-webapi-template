using AutoMapper;
using FluentValidation;
using MyBuyingList.Application.DTOs.UserDtos;
using MyBuyingList.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Contracts.BuyingListDtos;

//Since I'm using DTO for incoming and outcoming data, I made some properties nullable
//so the user does not specify them on Creating a BuyingList
public class CreateBuyingListDto
{
    public int? Id { get; set; }
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