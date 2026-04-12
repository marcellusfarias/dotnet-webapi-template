using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Options;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using MyBuyingList.Application.Common.Services;

namespace MyBuyingList.Application.Tests.Features.Login;

public class AuthServiceTests
{
    private readonly AuthService _sut;
    private readonly IFixture _fixture;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IJwtProvider _jwtProviderMock = Substitute.For<IJwtProvider>();
    private static readonly PasswordEncryptionService EncryptionService = new();
    
    private readonly IPasswordEncryptionService _passwordEncryptionService =
        Substitute.For<IPasswordEncryptionService>();

    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
    private readonly ILogger<AuthService> _loggerMock = Substitute.For<ILogger<AuthService>>();
    private readonly LockoutOptions _lockoutOptions = new() { MaxFailedAttempts = 5, LockoutDurationMinutes = 15 };

    private readonly RefreshTokenOptions _refreshTokenOptions =
        new() { ExpirationDays = 7, RevokedTokenRetentionDays = 30 };

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _userRepositoryMock,
            _jwtProviderMock,
            _passwordEncryptionService,
            _refreshTokenRepositoryMock,
            Options.Create(_lockoutOptions),
            Options.Create(_refreshTokenOptions),
            _loggerMock);

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

    #region Authenticate tests
    
    [Fact]
    public async Task Authenticate_ShouldReturnLoginResponse_WhenAuthenticationIsValid()
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
            .Returns(("custom_token", DateTimeOffset.UtcNow.AddSeconds(60)));

        //Act
        var result = await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        result.AccessToken.Should().Be("custom_token");
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.AccessTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
        result.RefreshTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Authenticate_ShouldThrowsException_WhenUserDoesNotExist()
    {
        //Arrange
        var dto = new LoginRequest
        {
            Password = _fixture.Create<string>(),
            Username = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(dto.Username, CancellationToken.None)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    [Fact]
    public async Task Authenticate_ShouldThrowsException_WhenPasswordsDontMatch()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 0)
            .Create();

        var dto = new LoginRequest
        {
            Password = _fixture.Create<string>(),
            Username = user.UserName
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage($"An error occurred when authenticating user {user.UserName.ToLower()}.");
    }

    [Fact]
    public async Task Authenticate_ShouldThrowException_WhenFailsToGenerateJwtToken()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 0)
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Password = _fixture.Create<string>(),
            Username = user.UserName
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, CancellationToken.None)
            .Throws(new DatabaseException(new Exception()));

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<DatabaseException>();
    }

    [Fact]
    public async Task Authenticate_ShouldThrowAccountLockedException_WhenAccountIsLocked()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, DateTime.UtcNow.AddMinutes(10))
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AccountLockedException>();
    }

    [Fact]
    public async Task Authenticate_ShouldThrowWithCorrectMinutesRemaining_WhenAccountIsAlreadyLocked()
    {
        //Arrange
        const int lockoutMinutes = 10;
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, DateTime.UtcNow.AddMinutes(lockoutMinutes))
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert — MinutesRemaining should be within [lockoutMinutes - 1, lockoutMinutes]
        // because Math.Ceiling is used and a negligible amount of time may elapse during the test.
        var exception = await act.Should().ThrowAsync<AccountLockedException>();
        exception.Which.MinutesRemaining.Should().BeInRange(lockoutMinutes - 1, lockoutMinutes);
    }

    [Fact]
    public async Task Authenticate_ShouldThrowWithCorrectMinutesRemaining_WhenAccountIsJustLocked()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, _lockoutOptions.MaxFailedAttempts - 1)
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert — MinutesRemaining must equal the configured LockoutDurationMinutes exactly.
        var exception = await act.Should().ThrowAsync<AccountLockedException>();
        exception.Which.MinutesRemaining.Should().Be(_lockoutOptions.LockoutDurationMinutes);
    }

    [Fact]
    public async Task Authenticate_ShouldIncrementCounter_WhenWrongPasswordBelowThreshold()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 2)
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AuthenticationException>();
        await _userRepositoryMock.Received(1).IncrementFailedLoginAttemptsAsync(user.Id, null, CancellationToken.None);
    }

    [Fact]
    public async Task Authenticate_ShouldLockAccount_WhenWrongPasswordAtThreshold()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, _lockoutOptions.MaxFailedAttempts - 1)
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AccountLockedException>();
        await _userRepositoryMock.Received(1)
            .IncrementFailedLoginAttemptsAsync(user.Id, Arg.Is<DateTime?>(d => d != null), CancellationToken.None);
    }

    [Fact]
    public async Task Authenticate_ShouldResetLockout_WhenCorrectPasswordAfterFailures()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 3)
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest
        {
            Username = user.UserName,
            Password = _fixture.Create<string>()
        };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, CancellationToken.None)
            .Returns(("custom_token", DateTimeOffset.UtcNow.AddSeconds(60)));

        //Act
        var result = await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        result.AccessToken.Should().Be("custom_token");
        await _userRepositoryMock.Received(1).ResetLockoutAsync(user.Id, CancellationToken.None);
    }

    [Fact]
    public async Task Authenticate_ShouldPersistRefreshToken_WhenAuthenticationSucceeds()
    {
        //Arrange
        var user = _fixture.Build<User>()
            .With(u => u.LockoutEnd, (DateTime?)null)
            .With(u => u.FailedLoginAttempts, 0)
            .Create();
        user.UserName = user.UserName.ToLower();

        var dto = new LoginRequest { Username = user.UserName, Password = _fixture.Create<string>() };

        _userRepositoryMock
            .GetActiveUserByUsernameAsync(user.UserName, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPassword(dto.Password, user.Password)
            .Returns(true);

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, CancellationToken.None)
            .Returns(("custom_token", DateTimeOffset.UtcNow.AddSeconds(60)));

        //Act
        await _sut.AuthenticateAsync(dto, CancellationToken.None);

        //Assert
        await _refreshTokenRepositoryMock.Received(1).AddAsync(
            Arg.Is<RefreshToken>(t => t.UserId == user.Id && !t.IsRevoked),
            CancellationToken.None);
    }

    #endregion

    #region Refresh tests

    [Fact]
    public async Task Refresh_ReturnsNewLoginResponseAndRevokeOldToken_WhenTokenIsValid()
    {
        // Arrange
        const int userId = 42;
        var (rawToken, storedToken) = BuildValidToken(userId);

        _refreshTokenRepositoryMock
            .GetByTokenHashAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(storedToken);

        _jwtProviderMock
            .GenerateTokenAsync(userId, CancellationToken.None)
            .Returns(("new_jwt", DateTimeOffset.UtcNow.AddSeconds(60)));

        var request = new RefreshRequest { RefreshToken = rawToken };

        // Act
        var result = await _sut.RefreshAsync(request, CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("new_jwt");
        result.RefreshToken.Should().NotBe(rawToken);
        result.AccessTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
        result.RefreshTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
        await _refreshTokenRepositoryMock.Received(1).RevokeAndAddAsync(
            storedToken,
            Arg.Is<RefreshToken>(t => t.UserId == userId && !t.IsRevoked),
            CancellationToken.None);
    }

    [Fact]
    public async Task Refresh_ThrowsAuthenticationException_WhenTokenNotFound()
    {
        // Arrange
        _refreshTokenRepositoryMock
            .GetByTokenHashAsync(Arg.Any<string>(), CancellationToken.None)
            .ReturnsNull();

        var request = new RefreshRequest { RefreshToken = "nonexistent" };

        // Act
        var act = async () => await _sut.RefreshAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    [Fact]
    public async Task Refresh_ThrowsAuthenticationException_WhenTokenIsExpired()
    {
        // Arrange
        var (rawToken, storedToken) = BuildValidToken(expiresAt: DateTimeOffset.UtcNow.AddDays(-1));

        _refreshTokenRepositoryMock
            .GetByTokenHashAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(storedToken);

        var request = new RefreshRequest { RefreshToken = rawToken };

        // Act
        var act = async () => await _sut.RefreshAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>();
    }

    [Fact]
    public async Task Refresh_RevokesAllUserTokensAndThrows_WhenRevokedTokenIsReused()
    {
        // Arrange
        const int userId = 42;
        var (rawToken, storedToken) = BuildValidToken(userId, isRevoked: true);

        _refreshTokenRepositoryMock
            .GetByTokenHashAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(storedToken);

        var request = new RefreshRequest { RefreshToken = rawToken };

        // Act
        var act = async () => await _sut.RefreshAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>();
        await _refreshTokenRepositoryMock.Received(1).RevokeAllForUserAsync(userId, CancellationToken.None);
        _loggerMock.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains(userId.ToString())),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Refresh_ShouldNotAddRefreshTokenIntoDatabase_WhenErrorGeneratingAuthenticationToken()
    {
        // Arrange
        const int userId = 42;
        var (rawToken, storedToken) = BuildValidToken(userId);

        _refreshTokenRepositoryMock
            .GetByTokenHashAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(storedToken);

        _jwtProviderMock
            .GenerateTokenAsync(userId, CancellationToken.None)
            .Throws(new DatabaseException(new Exception()));

        var request = new RefreshRequest { RefreshToken = rawToken };

        // Act
        var act = async () => await _sut.RefreshAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DatabaseException>();
        await _refreshTokenRepositoryMock.DidNotReceive().RevokeAndAddAsync(Arg.Any<RefreshToken>(), Arg.Any<RefreshToken>(), CancellationToken.None);
    }

    private static (string RawToken, RefreshToken Entity) BuildValidToken(int userId = 42, bool isRevoked = false,
        DateTimeOffset? expiresAt = null)
    {
        string token = Guid.NewGuid().ToString();
        RefreshToken entity = new RefreshToken()
        {
            UserId = userId,
            IsRevoked = isRevoked,
            User = new User { UserName = "testuser", Email = "test@example.com", Password = "hash", Active = true },
            ExpiresAt = expiresAt ?? DateTimeOffset.UtcNow.AddDays(7),
            CreatedAt = DateTimeOffset.UtcNow,
            TokenHash = EncryptionService.ComputeRefreshTokenHash(token)
        };

        return (token, entity);
    }
    
    #endregion

    
    
    
}
