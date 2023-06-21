using FluentValidation;
using MyBuyingList.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Validations
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator() 
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Please specify a name.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please specify an email address.");
        }
    }
}
