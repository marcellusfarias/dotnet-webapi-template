using FluentValidation;
using MyBuyingList.Application.Common.Helpers;
using MyBuyingList.Application.Features.Users.DTOs;
using System.Text.RegularExpressions;

namespace MyBuyingList.Application.Features.Users.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email).Must(ValidEmail);
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).Must(PasswordHelper.IsValidPassword);
    }

    private bool ValidEmail(string email)
    {
        Regex rxEmail = new Regex("/^[a-z0-9.]+@[a-z0-9]+\\.[a-z]+\\.([a-z]+)?$/i");
        return rxEmail.IsMatch(email);
    }
}
