using FluentValidation;
using MyBuyingList.Application.Features.Users.DTOs;

namespace MyBuyingList.Application.Features.Users.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).Must(ValidPassword);
    }

    private bool ValidPassword(string password)
    {
        return password.Length >= 8;
    }
}
