using FluentValidation;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Common.Helpers;
using MyBuyingList.Application.Features.Users.DTOs;

namespace MyBuyingList.Application.Features.Users.Validators;

public class UpdateUserPasswordDtoValidator : AbstractValidator<UpdateUserPasswordDto>
{
    public UpdateUserPasswordDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);

        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage(ValidationMessages.EMPTY_VALUE_ERROR)
            .Must(PasswordHelper.IsValidPassword)
            .WithMessage(ValidationMessages.INVALID_VALUE);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(ValidationMessages.EMPTY_VALUE_ERROR)
            .Must(PasswordHelper.IsValidPassword)
            .WithMessage(ValidationMessages.INVALID_VALUE);
    }
}
