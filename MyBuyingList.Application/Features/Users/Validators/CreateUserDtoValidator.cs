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
            .NotEmpty()
            .MaximumLength(FieldLengths.USER_EMAIL_MAX_LENGTH)
            .Must(ValidEmail)
            .WithMessage(ValidationMessages.InvalidEmail);            

        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(FieldLengths.USER_USERNAME_MIN_LENGTH)
            .MaximumLength(FieldLengths.USER_USERNAME_MAX_LENGTH)
            .Must(ValidUsername)
            .WithMessage(ValidationMessages.InvalidUsername);

        RuleFor(x => x.Password)
            .Must(PasswordHelper.IsValidPassword)
            .WithMessage(ValidationMessages.InvalidPassword);
    }

    private bool ValidEmail(string email)
    {
        Regex rxEmail = new Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
        return rxEmail.IsMatch(email);
    }

    private bool ValidUsername(string username)
    {
        Regex rx = new Regex("^[a-zA-Z0-9_]+$");
        return rx.IsMatch(username);
    }
}
