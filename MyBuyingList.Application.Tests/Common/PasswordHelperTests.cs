using MyBuyingList.Application.Common.Helpers;

namespace MyBuyingList.Application.Tests.Common;

public class PasswordHelperTests
{
    public PasswordHelperTests() { }

    [Theory]
    [InlineData("AvrT9d3!@")]
    [InlineData("Password123%")]
    [InlineData("123@#$%Ps")]
    [InlineData("Ps987654321@#$%&*+_()':;?.,![]\\-")]
    public void PasswordHelperIsValidPassword_ShouldReturnTrue_WhenPasswordIsValid(string password)
    {
        // Act
        var result = PasswordHelper.IsValidPassword(password);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("password")]
    [InlineData("P@0^pas")] // doesnt have 8 chars min
    [InlineData("")]
    [InlineData("Password123")] // no special character
    [InlineData("password123!")] // no upper case
    [InlineData("PASSWORD123!")] // no lower case
    [InlineData("PASSWORd!")] // no number
    [InlineData("Password123@VeryLongPasswordVeryVeryLongVeryLongNotTooLong")] // max 32 chars
    public void PasswordHelperIsValidPassword_ShouldReturnFalse_WhenPasswordIsNotValid(string password)
    {
        // Act
        var result = PasswordHelper.IsValidPassword(password);

        // Assert
        result.Should().BeFalse();
    }
}
