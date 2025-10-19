using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Validators;
using MyBuyingList.Application.Tests.TestUtils;
using MyBuyingList.Domain.Constants;
using System.Net.Mail;
using FluentValidation;
using FluentValidation.Resources;

namespace MyBuyingList.Application.Tests.Features.Users;

public class CreateUserDtoValidatorTests
{
    private readonly CreateUserDtoValidator _sut;
    private const string ValidPassword = "Password123%";
    private const string ValidEmail = "test@gmail.com";
    private const string ValidUsername = "sample_username";

    public CreateUserDtoValidatorTests()
    {
        _sut = new CreateUserDtoValidator();
        ValidatorOptions.Global.LanguageManager = new LanguageManager()
        {
            Culture = System.Globalization.CultureInfo.GetCultureInfo("en"),
        };
    }

    private static CreateUserDto CreateDto(string username, string email, string password)
    {
        return new CreateUserDto(username, email, password);
    }

    public static IEnumerable<object[]> ValidCreateUserDtos()
    {
        var fixture = new Fixture();

        // Testing username
        yield return [CreateDto("som", ValidEmail, ValidPassword)]; // length 3
        yield return [CreateDto("something_aleatory_wlength_32", ValidEmail, ValidPassword)]; // length 32
        yield return [CreateDto("NuMbeR_1234567890", ValidEmail, ValidPassword)]; // all numbers
        yield return [CreateDto("NuMbeR_____1234567890", ValidEmail, ValidPassword)]; // multiple underscores

        // Testing email
        yield return [CreateDto(ValidUsername, "validemail@gmail.com", ValidPassword)];
        yield return [CreateDto(ValidUsername, "V@BB.UE", ValidPassword)];
        yield return [CreateDto(ValidUsername, fixture.Create<MailAddress>().Address, ValidPassword)]; // length 3
        yield return [CreateDto(ValidUsername, "stuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz@example.com", ValidPassword)
        ]; // length 254

        // Passwords are tested on the PasswordHelperTests class
    }

    public static IEnumerable<object[]> InvalidCreateUserDtos()
    {
        // Invalid usernames
        yield return
        [
            CreateDto("so", ValidEmail, ValidPassword),
            FluentValidationHelper.GetMinLengthMessage("User Name", FieldLengths.USER_USERNAME_MIN_LENGTH, 2)
        ];

        yield return
        [
            CreateDto("something_aleatory_greater_than_length_32", ValidEmail, ValidPassword),
            FluentValidationHelper.GetMaxLengthMessage("User Name", FieldLengths.USER_USERNAME_MAX_LENGTH, 41)
        ];

        yield return
        [
            CreateDto("", ValidEmail, ValidPassword),
            FluentValidationHelper.GetNotEmptyFieldMessage("User Name")
        ];

        yield return
        [
            CreateDto("invalid!@#$", ValidEmail, ValidPassword),
            ValidationMessages.InvalidUsername
        ];

        // Invalid email
        yield return
        [
            CreateDto(ValidUsername, "", ValidPassword),
             FluentValidationHelper.GetNotEmptyFieldMessage("Email")
        ];

        yield return
        [
            CreateDto(ValidUsername, "astuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz@example.com", ValidPassword),
            FluentValidationHelper.GetMaxLengthMessage("Email", FieldLengths.USER_EMAIL_MAX_LENGTH, 255)
        ];

        yield return
        [
            CreateDto(ValidUsername, "a##@gmail.com", ValidPassword),
            ValidationMessages.InvalidEmail
        ];

        yield return
        [
            CreateDto(ValidUsername, "abc@.com", ValidPassword),
            ValidationMessages.InvalidEmail
        ];

        yield return
        [
            CreateDto(ValidUsername, "abc@gmail.", ValidPassword),
            ValidationMessages.InvalidEmail
        ];

        yield return
        [
            CreateDto(ValidUsername, "abc@gmail", ValidPassword),
            ValidationMessages.InvalidEmail
        ];

        // Invalid password
        yield return
        [
            CreateDto(ValidUsername, ValidEmail, "123"),
            ValidationMessages.InvalidPassword
        ];
    }

    [Theory]
    [MemberData(nameof(ValidCreateUserDtos))]
    public void Validate_ShouldReturnSuccess_WhenThereAreNoErrors(CreateUserDto dto)
    {
        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidCreateUserDtos))]
    public void Validate_ShouldReturnError_WhenThereAreErrors(CreateUserDto dto, string errorMessage)
    {
        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be(errorMessage);
    }
}
