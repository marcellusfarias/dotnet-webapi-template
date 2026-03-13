using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Options;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace MyBuyingList.Application.Tests.Features.Login;

public class LoginServiceTests
{
    private readonly LoginService _sut;
    private readonly IFixture _fixture;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IJwtProvider _jwtProviderMock = Substitute.For<IJwtProvider>();
    private readonly IPasswordEncryptionService _passwordEncryptionService = Substitute.For<IPasswordEncryptionService>();
    private readonly LockoutOptions _lockoutOptions = new() { MaxFailedAttempts = 5, LockoutDurationMinutes = 15 };

    public LoginServiceTests()
    {
        _sut = new LoginService(_userRepositoryMock, _jwtProviderMock, _passwordEncryptionService, Options.Create(_lockoutOptions));
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<User>(c => c
            .With(x =>
                x.Email,
                _fixture.Create<MailAddress>().Address));

        _fixture.Customize<CreateUserRequest>(c => c
            .With(x =>
                x.Email,
                _fixture.Create<MailAddress>().Address));
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldReturnToken_WhenAuthenticationIsValid()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 0)
            .Create();
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = attemptingPassword
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(attemptingPassword, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, CancellationToken.None)
            .Returns("custom_token");

        //Act
        string token = await _sut.AuthenticateAndReturnJwtTokenAsync(dto, CancellationToken.None);

        //Assert
        token.Should().BeEquivalentTo("custom_token");
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldThrowsException_WhenUserDoesNotExist()
    {
        //Arrange
        var attemptingPassword = _fixture.Create<string>();
        var attemptingUserName = _fixture.Create<string>();

        var loginDto = new LoginRequest
        {
            Password = attemptingPassword,
            Username = attemptingUserName
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(attemptingUserName, CancellationToken.None)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>($"An error occured when authenticating user {attemptingUserName}.");
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldThrowsException_WhenPasswordsDontMatch()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 0)
            .Create();
        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginRequest
        {
            Password = attemptingPassword,
            Username = user.UserName
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(attemptingPassword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>().WithMessage($"An error occurred when authenticating user {user.UserName.ToLower()}.");
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldThrowException_WhenFailsToGenerateJwtToken()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 0)
            .Create();
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginRequest
        {
            Password = attemptingPassword,
            Username = user.UserName
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(attemptingPassword, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, CancellationToken.None)
            .Throws(new DatabaseException(new Exception()));

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<DatabaseException>();
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldThrowAccountLockedException_WhenAccountIsLocked()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, DateTime.UtcNow.AddMinutes(10))
            .Create();
        user.UserName = user.UserName.ToLower();

        var loginDto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AccountLockedException>();
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldThrowWithCorrectMinutesRemaining_WhenAccountIsAlreadyLocked()
    {
        //Arrange
        const int lockoutMinutes = 10;
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, DateTime.UtcNow.AddMinutes(lockoutMinutes))
            .Create();
        user.UserName = user.UserName.ToLower();

        var loginDto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert — MinutesRemaining should be within [lockoutMinutes - 1, lockoutMinutes]
        // because Math.Ceiling is used and a negligible amount of time may elapse during the test.
        var exception = await act.Should().ThrowAsync<AccountLockedException>();
        exception.Which.MinutesRemaining.Should().BeInRange(lockoutMinutes - 1, lockoutMinutes);
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldThrowWithCorrectMinutesRemaining_WhenAccountIsJustLocked()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, _lockoutOptions.MaxFailedAttempts - 1)
            .Create();
        user.UserName = user.UserName.ToLower();

        var loginDto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(loginDto.Password, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert — MinutesRemaining must equal the configured LockoutDurationMinutes exactly.
        var exception = await act.Should().ThrowAsync<AccountLockedException>();
        exception.Which.MinutesRemaining.Should().Be(_lockoutOptions.LockoutDurationMinutes);
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldIncrementCounter_WhenWrongPasswordBelowThreshold()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 2)
            .Create();
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginRequest
        {
            Username = user.UserName,
            Password = attemptingPassword
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(attemptingPassword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>();
        await _userRepositoryMock.Received(1).IncrementFailedLoginAttemptsAsync(user.Id, null, CancellationToken.None);
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldLockAccount_WhenWrongPasswordAtThreshold()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, _lockoutOptions.MaxFailedAttempts - 1)
            .Create();
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginRequest
        {
            Username = user.UserName,
            Password = attemptingPassword
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(attemptingPassword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AccountLockedException>();
        await _userRepositoryMock.Received(1).IncrementFailedLoginAttemptsAsync(user.Id, Arg.Is<DateTime?>(d => d != null), CancellationToken.None);
    }

    [Fact]
    public async Task AuthenticateAndReturnJwtToken_ShouldResetLockout_WhenCorrectPasswordAfterFailures()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 3)
            .Create();
        user.UserName = user.UserName.ToLower();

        var attemptingPassword = _fixture.Create<string>();

        var loginDto = new LoginRequest
        {
            Username = user.UserName,
            Password = attemptingPassword
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(attemptingPassword, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, CancellationToken.None)
            .Returns("custom_token");

        //Act
        string token = await _sut.AuthenticateAndReturnJwtTokenAsync(loginDto, CancellationToken.None);

        //Assert
        token.Should().BeEquivalentTo("custom_token");
        await _userRepositoryMock.Received(1).ResetLockoutAsync(user.Id, CancellationToken.None);
    }
}
