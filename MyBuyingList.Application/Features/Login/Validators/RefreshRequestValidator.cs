using FluentValidation;
using MyBuyingList.Application.Features.Login.DTOs;

namespace MyBuyingList.Application.Features.Login.Validators;

public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
