using FluentValidation;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.BuyingLists.DTOs;
using MyBuyingList.Domain.Constants;

namespace MyBuyingList.Application.Features.BuyingLists.Validators;

public class UpdateBuyingListNameValidator : AbstractValidator<UpdateBuyingListNameDto>
{
    public UpdateBuyingListNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.EMPTY_VALUE_ERROR)
            .MaximumLength(FieldLengths.BUYINGLIST_NAME_MAX_LENGTH)
            .WithMessage(ValidationMessages.MAX_LENGTH_ERROR);
    }
}
