using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Validators;
using MyBuyingList.Application.Tests.TestUtils;

namespace MyBuyingList.Application.Tests.Features.Users;

public class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _sut;
    private const string ValidPassword = "Password123%";

    public ChangePasswordRequestValidatorTests()
    {
        _sut = new ChangePasswordRequestValidator();
    }

    private static ChangePasswordRequest CreateDto(string oldPassword, string newPassword)
    {
        return new ChangePasswordRequest(oldPassword, newPassword);
    }

    public static IEnumerable<object[]> ValidDtos()
    {
        yield return [CreateDto(ValidPassword, ValidPassword)];
    }

    public static IEnumerable<object[]> InvalidDtos()
    {
        // Testing passwords
        yield return
        [
            CreateDto("", ValidPassword),
            FluentValidationHelper.GetNotEmptyFieldMessage("Old Password")
        ];

        yield return
        [
            CreateDto(ValidPassword, ""),
            FluentValidationHelper.GetNotEmptyFieldMessage("New Password")
        ];

        yield return
        [
            CreateDto("1234", ValidPassword),
            ValidationMessages.InvalidPassword
        ];

        yield return
        [
            CreateDto(ValidPassword, "12345"),
            ValidationMessages.InvalidPassword
        ];
    }

    [Theory]
    [MemberData(nameof(ValidDtos))]
    public void Validate_ShouldReturnSuccess_WhenThereAreNoErrors(ChangePasswordRequest dto)
    {
        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidDtos))]
    public void Validate_ShouldReturnError_WhenThereAreErrors(ChangePasswordRequest dto, string errorMessage)
    {
        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be(errorMessage);
    }

}
