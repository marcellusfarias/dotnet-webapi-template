using FluentValidation;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Groups.DTOs;
using MyBuyingList.Domain.Constants;

namespace MyBuyingList.Application.Features.Groups.Validators;

public class UpdateGroupNameValidator : AbstractValidator<UpdateGroupNameDto>
{
    public UpdateGroupNameValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty()
            .WithMessage(ValidationMessages.EMPTY_VALUE_ERROR)
            .MaximumLength(FieldLengths.GROUP_NAME_MAX_LENGTH)
            .WithMessage(ValidationMessages.MIN_LENGTH_ERROR);
    }
}