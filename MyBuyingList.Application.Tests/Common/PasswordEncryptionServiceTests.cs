using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Services;

namespace MyBuyingList.Application.Tests.Common;

public class PasswordEncryptionServiceTests
{
    private readonly PasswordEncryptionService _sut;

    private const string LongPassword = "12345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678";

    public PasswordEncryptionServiceTests()
    {
        _sut = new PasswordEncryptionService();
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345678")]
    [InlineData(LongPassword)]
    public void HashPassword_MustReturnValidHashedPassword_WhenPasswordIsNotValid(string password)
    {
        // Act
        var result = _sut.HashPassword(password);

        // Assure
        result.Should().StartWith("$2a$");
        result.Should().HaveLength(60); // not sure about this. Monitor database column and adjust as needed
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("MyHardPassword123", "")]
    [InlineData("MyHardPassword123", "VshqThLS2w0/9W02/3RnfOo6Wy7Evsb8VvQnUs7TC89b0dMuBSpFi")]
    public void VerifyPassword_MustThrowException_WhenHashIsInvalid(string password, string hashedPassword)
    {
        // Act
        var action = _sut.Invoking(_ => _sut.VerifyPasswordsAreEqual(password, hashedPassword));

        // Assure
        action.Should().Throw<InternalServerErrorException>();
    }

    [Theory]
    [InlineData("", "$2a$12$VshqThLS2w0/9W02/3RnfOo6Wy7Evsb8VvQnUs7TC89b0dMuBSpFi", false)]
    [InlineData("12345678", "$2a$12$VshqThLS2w0/9W02/3RnfOo6Wy7Evsb8VvQnUs7TC89b0dMuBSpFi", false)]
    [InlineData(LongPassword, "$2a$12$VshqThLS2w0/9W02/3RnfOo6Wy7Evsb8VvQnUs7TC89b0dMuBSpFi", false)]
    [InlineData("12345678", "$2a$12$zIdWbQXg/ZnKc4bkK/pxtuAzChQQnMAgyXYaO2F/AQY2CWHHc0dB.", true)]
    public void VerifyPassword_MustVerifyCorrectly_WhenParametersAreValid(string password, string hashedPassword, bool expectedResult)
    {
        // Act
        var result = _sut.VerifyPasswordsAreEqual(password, hashedPassword);

        // Assure
        result.Should().Be(expectedResult);
    }
}
