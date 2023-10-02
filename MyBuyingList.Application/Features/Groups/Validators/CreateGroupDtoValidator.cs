using FluentValidation;
using MyBuyingList.Application.Features.Groups.DTOs;

namespace MyBuyingList.Application.Features.Groups.Validators;

public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
{
    public CreateGroupDtoValidator()
    {
        RuleFor(x => x.GroupName).NotEmpty().MaximumLength(256);
    }
}
