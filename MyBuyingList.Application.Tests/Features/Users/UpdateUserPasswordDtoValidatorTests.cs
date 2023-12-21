using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Validators;
using MyBuyingList.Application.Tests.TestUtils;

namespace MyBuyingList.Application.Tests.Features.Users;

public class UpdateUserPasswordDtoValidatorTests
{
    private UpdateUserPasswordDtoValidator _sut;
    private static readonly string VALID_PASSWORD = "Password123%";

    public UpdateUserPasswordDtoValidatorTests()
    {
        _sut = new UpdateUserPasswordDtoValidator();
    }

    private static UpdateUserPasswordDto CreateDto(int id, string oldPassword, string newPassword)
    {
        return new UpdateUserPasswordDto()
        {
            Id = id,
            OldPassword = oldPassword,
            NewPassword = newPassword
        };
    }

    public static IEnumerable<object[]> ValidDtos()
    {
        // Testing id
        yield return new object[] { CreateDto(1, VALID_PASSWORD, VALID_PASSWORD), };
        yield return new object[] { CreateDto(99999999, VALID_PASSWORD, VALID_PASSWORD), };
    }

    public static IEnumerable<object[]> InvalidDtos()
    {
        // Testing id
        yield return new object[]
        {
            CreateDto(0, VALID_PASSWORD, VALID_PASSWORD),
            FluentValidationHelper.GetNotEmptyFieldMessage("Id")
        };

        yield return new object[]
        {
            CreateDto(-1, VALID_PASSWORD, VALID_PASSWORD),
            FluentValidationHelper.GetFieldGreaterThanMessage("Id", 0)
        };

        // Testing passwords
        yield return new object[]
        {
            CreateDto(1, "", VALID_PASSWORD),
            FluentValidationHelper.GetNotEmptyFieldMessage("Old Password")
        };

        yield return new object[]
        {
            CreateDto(1, VALID_PASSWORD, ""),
            FluentValidationHelper.GetNotEmptyFieldMessage("New Password")
        };

        yield return new object[]
        {
            CreateDto(1, "1234", VALID_PASSWORD),
            ValidationMessages.INVALID_PASSWORD
        };

        yield return new object[]
        {
            CreateDto(1, VALID_PASSWORD, "12345"),
            ValidationMessages.INVALID_PASSWORD
        };
    }

    [Theory]
    [MemberData(nameof(ValidDtos))]
    public void Validate_ShouldReturnSuccess_WhenThereAreNoErrors(UpdateUserPasswordDto dto)
    {
        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidDtos))]
    public void Validate_ShouldReturnError_WhenThereAreErrors(UpdateUserPasswordDto dto, string errorMessage)
    {
        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be(errorMessage);
    }

}
