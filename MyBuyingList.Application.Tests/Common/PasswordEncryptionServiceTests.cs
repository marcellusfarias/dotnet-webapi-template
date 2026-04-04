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
        var action = _sut.Invoking(_ => _sut.VerifyPassword(password, hashedPassword));

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
        var result = _sut.VerifyPassword(password, hashedPassword);

        // Assure
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void HashPassword_BcryptTruncatesInputAt72Bytes()
    {
        // Passwords grow in 10-char steps, each step using a different character
        // so content genuinely differs up to the 72-byte boundary.
        var p52 = new string('A', 52);
        var p62 = new string('A', 52) + new string('B', 10);
        var p72 = new string('A', 52) + new string('B', 10) + new string('C', 10);
        // p82 shares its first 72 chars with p72 — only chars 73-82 differ.
        var p82 = new string('A', 52) + new string('B', 10) + new string('C', 10) + new string('D', 10);

        var hash52 = _sut.HashPassword(p52);
        var hash62 = _sut.HashPassword(p62);
        var hash72 = _sut.HashPassword(p72);

        // Within 72 bytes: distinct passwords do not cross-verify.
        _sut.VerifyPassword(p62, hash52).Should().BeFalse();
        _sut.VerifyPassword(p72, hash62).Should().BeFalse();

        // Beyond 72 bytes: BCrypt silently ignores chars 73+, so p82 is treated
        // identically to p72. This is why PasswordValidator enforces a 72-char max.
        _sut.VerifyPassword(p82, hash72).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ReturnsTrueForPasswordsDifferingOnlyBeyond72Chars()
    {
        // A password of exactly 72 chars and one with an extra char appended
        // are indistinguishable to BCrypt due to its 72-byte input truncation.
        var password72 = new string('A', 72);
        var password73 = new string('A', 72) + "X";

        var hash = _sut.HashPassword(password72);

        _sut.VerifyPassword(password73, hash).Should().BeTrue();
    }
}
