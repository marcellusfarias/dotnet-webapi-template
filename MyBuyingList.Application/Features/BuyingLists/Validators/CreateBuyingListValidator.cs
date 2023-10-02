using FluentValidation;
using MyBuyingList.Application.Features.BuyingLists.DTOs;

namespace MyBuyingList.Application.Features.BuyingLists.Validators;

public class CreateBuyingListValidator : AbstractValidator<CreateBuyingListDto>
{
    public CreateBuyingListValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Please specify a name.");
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .WithMessage("Please specify a group.");
    }
}
