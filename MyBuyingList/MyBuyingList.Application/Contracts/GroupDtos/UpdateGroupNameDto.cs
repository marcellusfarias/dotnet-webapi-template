using FluentValidation;
using MyBuyingList.Application.Contracts.BuyingListDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Contracts.GroupDtos;

public class UpdateGroupNameDto
{
    public required int Id { get; set; }
    public required string GroupName { get; set; }
}


public class UpdateGroupNameValidator : AbstractValidator<UpdateGroupNameDto>
{
    public UpdateGroupNameValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty()
            .WithMessage("Please specify a name.")
            .MaximumLength(256)
            .WithMessage("Name length shoulb be less or equal to 256.");
    }
}