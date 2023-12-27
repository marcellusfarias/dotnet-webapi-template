using FluentValidation;
using MyBuyingList.Application.Features.Login.DTOs;

namespace MyBuyingList.Application.Features.Login.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
