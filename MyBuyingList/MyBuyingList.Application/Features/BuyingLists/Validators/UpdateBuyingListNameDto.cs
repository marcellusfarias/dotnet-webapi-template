using FluentValidation;
using MyBuyingList.Application.Features.BuyingLists.DTOs;

namespace MyBuyingList.Application.Features.BuyingLists.Validators;

public class UpdateBuyingListNameValidator : AbstractValidator<UpdateBuyingListNameDto>
{
    public UpdateBuyingListNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Please specify a name.")
            .MaximumLength(256)
            .WithMessage("Name length shoulb be less or equal to 256.");
    }
}
