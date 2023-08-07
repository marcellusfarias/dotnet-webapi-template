using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.DTOs;

//Since I'm using DTO for incoming and outcoming data, I made some properties nullable
//so the user does not specify them on Creating a BuyingList
public class BuyingListDto
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public required int GroupId { get; set; }
    public int? CreatedBy { get; set; } 
    public DateTime? CreatedAt { get; set; } 
}

public class BuyingListValidator : AbstractValidator<BuyingListDto>
{
    public BuyingListValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Please specify a name.");
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .WithMessage("Please specify a group.");
    }
}