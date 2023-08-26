using AutoMapper;
using FluentValidation;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.DTOs.UserDtos;

public class CreateUserDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).Must(ValidPassword);
    }

    private bool ValidPassword(string password)
    {
        return password.Length >= 8;
    }
}

[AutoMapperMapping]
public class CreateUserDtoMapping
{
    public void ConfigureMappings(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<CreateUserDto, User>();
            //.ForMember(dest => dest.Active, o => o.MapFrom(src => true));
    }
}
