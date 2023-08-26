using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Contracts.BuyingListDtos;

public class UpdateBuyingListNameDto
{
    public required int Id { get;set; }
    public required string Name { get; set; }
}


public class UpdateBuyingListNameValidator : AbstractValidator<UpdateBuyingListNameDto>
{
    public UpdateBuyingListNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Please specify a name.")
            .MaximumLength(256)
            .WithMessage("Name length shoulb be less or equal to 256.");
    }
}