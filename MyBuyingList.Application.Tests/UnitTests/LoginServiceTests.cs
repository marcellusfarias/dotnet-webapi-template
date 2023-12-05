using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;
using System.Net.Mail;

namespace MyBuyingList.Application.Tests.UnitTests;

public class LoginServiceTests
{
    private LoginService _sut;
    private IFixture _fixture;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IJwtProvider _jwtProviderMock = Substitute.For<IJwtProvider>();
    private readonly IPasswordEncryptionService _passwordEncryptionService = Substitute.For<IPasswordEncryptionService>();
    public LoginServiceTests()
    {
        _sut = new LoginService(_userRepositoryMock, _jwtProviderMock, _passwordEncryptionService);
        _fixture = new Fixture();//.Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<User>(c => c
            .With(x =>
                x.Email,
                _fixture.Create<MailAddress>().Address));

        _fixture.Customize<CreateUserDto>(c => c
            .With(x =>
                x.Email,
                _fixture.Create<MailAddress>().Address));
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldReturnToken_WhenAuthenticationIsValid()
    {
        //Arrange
        var user = _fixture.Create<User>();
        var attemptingPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetActiveUserByUsername(user.UserName, default)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(attemptingPassword, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, default)
            .Returns("custom_token");

        //Act
        string token = await _sut.AuthenticateAndReturnJwtTokenAsync(user.UserName, attemptingPassword, default);

        //Assert
        token.Should().BeEquivalentTo("custom_token");
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("", "12345678")]
    [InlineData("username", "")]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowException_WhenMissingUsernameOrPassword(string username, string attemptingPassword)
    {
        //Arrange

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(username, attemptingPassword, default);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowsException_WhenUserDoesNotExist()
    {
        //Arrange
       var attemptingPassword = _fixture.Create<string>();
       var attemptingUserName = _fixture.Create<string>();

        _userRepositoryMock
            .GetActiveUserByUsername(attemptingUserName, default)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(attemptingUserName, attemptingPassword, default);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowsException_WhenPasswordsDontMatch()
    {
        //Arrange
        var user = _fixture.Create<User>();
        var attemptingPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetActiveUserByUsername(user.UserName, default)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(attemptingPassword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(user.UserName, attemptingPassword, default);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowException_WhenFailsToGenerateJwtToken()
    {
        //Arrange
        var user = _fixture.Create<User>();
        var attemptingPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetActiveUserByUsername(user.UserName, default)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(attemptingPassword, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, default)
            .Throws(new DatabaseException(new Exception()));

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(user.UserName, attemptingPassword, default);

        //Assert
        await act.Should().ThrowAsync<DatabaseException>();
    }
}
