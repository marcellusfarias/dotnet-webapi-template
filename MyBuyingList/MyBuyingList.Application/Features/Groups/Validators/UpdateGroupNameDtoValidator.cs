using FluentValidation;
using MyBuyingList.Application.Features.Groups.DTOs;

namespace MyBuyingList.Application.Features.Groups.Validators;

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