using FluentValidation;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Common.Helpers;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Constants;
using MyBuyingList.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace MyBuyingList.Application.Features.Users.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly Regex UsernameRegex = new("^[a-zA-Z0-9_]+$");

    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(FieldLengths.USER_EMAIL_MAX_LENGTH)
            .Must(EmailAddress.IsValid)
            .WithMessage(ValidationMessages.InvalidEmail);

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(FieldLengths.USER_USERNAME_MIN_LENGTH)
            .MaximumLength(FieldLengths.USER_USERNAME_MAX_LENGTH)
            .Must(ValidUsername)
            .WithMessage(ValidationMessages.InvalidUsername);

        RuleFor(x => x.Password)
            .Must(PasswordValidator.IsValidPassword)
            .WithMessage(ValidationMessages.InvalidPassword);
    }

    private static bool ValidUsername(string username) => UsernameRegex.IsMatch(username);
}
