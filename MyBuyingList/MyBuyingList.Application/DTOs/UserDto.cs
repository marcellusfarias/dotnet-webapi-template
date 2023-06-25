using FluentValidation;
using FluentValidation.Results;
using MyBuyingList.Application.Common.Exceptions;
//using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyBuyingList.Application.DTOs;

public class UserDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }    
    public required bool Active { get; set; }
}

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an id.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Please specify an email address.");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Please specify a name.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Please specify a password.");
        RuleFor(x => x.Active).NotEmpty().WithMessage("Please specify if it's active or not.");        
    }
}