using FluentValidation;
using MyBuyingList.Application.Features.Users.DTOs;

namespace MyBuyingList.Application.Features.Users.Validators;

public class UpdateUserPasswordDtoValidator : AbstractValidator<UpdateUserPasswordDto>
{
    public UpdateUserPasswordDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
        RuleFor(x => x.OldPassword).NotEmpty().MinimumLength(8);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}
