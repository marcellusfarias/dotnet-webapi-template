using FluentValidation;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Common.Helpers;
using MyBuyingList.Application.Features.Users.DTOs;

namespace MyBuyingList.Application.Features.Users.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .Must(PasswordValidator.IsValidPassword)
            .WithMessage(ValidationMessages.InvalidPassword);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Must(PasswordValidator.IsValidPassword)
            .WithMessage(ValidationMessages.InvalidPassword);
    }
}
