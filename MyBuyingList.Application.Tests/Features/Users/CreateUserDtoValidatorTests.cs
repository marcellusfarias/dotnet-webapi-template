using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Validators;
using MyBuyingList.Application.Tests.TestUtils;
using MyBuyingList.Domain.Constants;
using System.Net.Mail;

namespace MyBuyingList.Application.Tests.Features.Users;

public class CreateUserDtoValidatorTests
{
    private CreateUserDtoValidator _sut;
    private static readonly string VALID_PASSWORD = "Password123%";
    private static readonly string VALID_EMAIL = "test@gmail.com";
    private static readonly string VALID_USERNAME = "sample_username";

    public CreateUserDtoValidatorTests()
    {
        _sut = new CreateUserDtoValidator();
    }

    private static CreateUserDto CreateDto(string username, string email, string password)
    {
        return new CreateUserDto(username, email, password);
    }

    public static IEnumerable<object[]> ValidCreateUserDtos()
    {
        var fixture = new Fixture();

        // Testing username
        yield return new object[] { CreateDto("som", VALID_EMAIL, VALID_PASSWORD), }; // length 3
        yield return new object[] { CreateDto("something_aleatory_wlength_32", VALID_EMAIL, VALID_PASSWORD), }; // length 32
        yield return new object[] { CreateDto("NuMbeR_1234567890", VALID_EMAIL, VALID_PASSWORD), }; // all numbers
        yield return new object[] { CreateDto("NuMbeR_____1234567890", VALID_EMAIL, VALID_PASSWORD), }; // multiple underscores

        // Testing email
        yield return new object[] { CreateDto(VALID_USERNAME, "validemail@gmail.com", VALID_PASSWORD), };
        yield return new object[] { CreateDto(VALID_USERNAME, "V@BB.UE", VALID_PASSWORD), };
        yield return new object[] { CreateDto(VALID_USERNAME, fixture.Create<MailAddress>().Address, VALID_PASSWORD), }; // length 3
        yield return new object[] { CreateDto(VALID_USERNAME, "stuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz@example.com", VALID_PASSWORD), }; // length 254

        // Passwords are tested on the PasswordHelperTests class
    }

    public static IEnumerable<object[]> InvalidCreateUserDtos()
    {
        // Invalid usernames
        yield return new object[]
        {
            CreateDto("so", VALID_EMAIL, VALID_PASSWORD),
            FluentValidationHelper.GetMinLengthMessage("User Name", FieldLengths.USER_USERNAME_MIN_LENGTH, 2)
        };

        yield return new object[]
        {
            CreateDto("something_aleatory_greater_than_length_32", VALID_EMAIL, VALID_PASSWORD),
            FluentValidationHelper.GetMaxLengthMessage("User Name", FieldLengths.USER_USERNAME_MAX_LENGTH, 41)
        };

        yield return new object[]
        {
            CreateDto("", VALID_EMAIL, VALID_PASSWORD),
            FluentValidationHelper.GetNotEmptyFieldMessage("User Name")
        };

        yield return new object[]
        {
            CreateDto("invalid!@#$", VALID_EMAIL, VALID_PASSWORD),
            ValidationMessages.INVALID_USERNAME
        };

        // Invalid email
        yield return new object[]
        {
            CreateDto(VALID_USERNAME, "", VALID_PASSWORD),
             FluentValidationHelper.GetNotEmptyFieldMessage("Email")
        };

        yield return new object[]
        {
            CreateDto(VALID_USERNAME, "astuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz@example.com", VALID_PASSWORD),
            FluentValidationHelper.GetMaxLengthMessage("Email", FieldLengths.USER_EMAIL_MAX_LENGTH, 255)
        };

        yield return new object[]
        {
            CreateDto(VALID_USERNAME, "a##@gmail.com", VALID_PASSWORD),
            ValidationMessages.INVALID_EMAIL
        };

        yield return new object[]
        {
            CreateDto(VALID_USERNAME, "abc@.com", VALID_PASSWORD),
            ValidationMessages.INVALID_EMAIL
        };

        yield return new object[]
        {
            CreateDto(VALID_USERNAME, "abc@gmail.", VALID_PASSWORD),
            ValidationMessages.INVALID_EMAIL
        };

        yield return new object[]
        {
            CreateDto(VALID_USERNAME, "abc@gmail", VALID_PASSWORD),
            ValidationMessages.INVALID_EMAIL
        };

        // Invalid password
        yield return new object[]
        {
            CreateDto(VALID_USERNAME, VALID_EMAIL, "123"),
            ValidationMessages.INVALID_PASSWORD
        };
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
