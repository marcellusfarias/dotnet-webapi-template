using FluentValidation;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Common.Helpers;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Constants;
using System.Text.RegularExpressions;

namespace MyBuyingList.Application.Features.Users.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .Must(ValidEmail)
            .WithMessage(ValidationMessages.INVALID_VALUE);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage(ValidationMessages.EMPTY_VALUE_ERROR)
            .MinimumLength(FieldLengths.USER_USERNAME_MIN_LENGTH)
            .WithMessage(ValidationMessages.MIN_LENGTH_ERROR)
            .MaximumLength(FieldLengths.USER_USERNAME_MAX_LENGTH)
            .WithMessage(ValidationMessages.MAX_LENGTH_ERROR);

        RuleFor(x => x.Password)
            .Must(PasswordHelper.IsValidPassword)
            .WithMessage(ValidationMessages.INVALID_VALUE);
    }

    private bool ValidEmail(string email)
    {
        Regex rxEmail = new Regex("/^[a-z0-9.]+@[a-z0-9]+\\.[a-z]+\\.([a-z]+)?$/i");
        return rxEmail.IsMatch(email);
    }
}
