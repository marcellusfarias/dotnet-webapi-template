using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;
using System.Net.Mail;

namespace MyBuyingList.Application.Tests.Features.Login;

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
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var dto = new LoginDto
        {
            Username = user.UserName,
            Password = attemptingPassword
        };

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
        string token = await _sut.AuthenticateAndReturnJwtTokenAsync(dto, default);

        //Assert
        token.Should().BeEquivalentTo("custom_token");
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowsException_WhenUserDoesNotExist()
    {
        //Arrange
        var attemptingPassword = _fixture.Create<string>();
        var attemptingUserName = _fixture.Create<string>();

        var loginDto = new LoginDto
        {
            Password = attemptingPassword,
            Username = attemptingUserName
        };

        _userRepositoryMock
            .GetActiveUserByUsername(attemptingUserName, default)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, default);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>($"An error occured when authenticating user {attemptingUserName}.");
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowsException_WhenPasswordsDontMatch()
    {
        //Arrange
        var user = _fixture.Create<User>();
        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginDto
        {
            Password = attemptingPassword,
            Username = user.UserName
        };

        _userRepositoryMock
            .GetActiveUserByUsername(user.UserName, default)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(attemptingPassword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, default);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>().WithMessage($"An error occured when authenticating user {user.UserName.ToLower()}."); ;
    }

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ShouldThrowException_WhenFailsToGenerateJwtToken()
    {
        //Arrange
        var user = _fixture.Create<User>();
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginDto
        {
            Password = attemptingPassword,
            Username = user.UserName
        };

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
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, default);

        //Assert
        await act.Should().ThrowAsync<DatabaseException>();
    }
}
