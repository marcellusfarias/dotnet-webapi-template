using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Json;

namespace MyBuyingList.Web.Tests.IntegrationTests;

// TODO: test RateLimiting
public class AuthControllerIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public AuthControllerIntegrationTests(ResourceFactory resourceFactory)
        : base(resourceFactory)
    {
        _client = resourceFactory.HttpClient;
    }

    #region Authenticate
    
    [Fact]
    public async Task Authenticate_ReturnsLoginResponse_WhenCredentialsAreValid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        var loginDto = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = Utils.TestUserPassword
        };

        // Act
        var response = await _client.PostAsync(Constants.AddressAuthenticationEndpoint, Utils.GetJsonContentFromObject(loginDto), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(_cancellationToken);
        loginResponse.Should().NotBeNull();
        loginResponse!.AccessToken.Should().NotBeNullOrEmpty();
        loginResponse.RefreshToken.Should().NotBeNullOrEmpty();
        // AccessTokenExpiresAt is computed server-side from JwtSettings.ExpirationTime;
        // assert it was set recently rather than asserting a specific future offset.
        loginResponse.AccessTokenExpiresAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(6));
        loginResponse.RefreshTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Authenticate_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        var loginDto = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = string.Concat(Utils.TestUserPassword, ".")
        };

        // Act
        var response = await _client.PostAsync(Constants.AddressAuthenticationEndpoint, Utils.GetJsonContentFromObject(loginDto), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Authenticate_ReturnsUnauthorized_WithLockoutMessage_WhenAccountIsLocked()
    {
        // Arrange — create a test user and exhaust MaxFailedAttempts (3 in test config)
        await Utils.InsertTestUser(_client);

        var loginDtoWrong = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = string.Concat(Utils.TestUserPassword, ".")
        };

        var url = Constants.AddressAuthenticationEndpoint;

        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoWrong), _cancellationToken);
        }

        // Act — one more attempt (with wrong password)
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoWrong), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync(_cancellationToken);
        body.Should().Contain("temporarily locked");
    }

    [Fact]
    public async Task Authenticate_ReturnsUnauthorized_WithLockoutMessage_WhenAccountIsLockedEvenWithCorrectPassword()
    {
        // Arrange — create test user and trigger lockout
        await Utils.InsertTestUser(_client);

        var loginDtoWrong = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = string.Concat(Utils.TestUserPassword, ".")
        };

        var url = Constants.AddressAuthenticationEndpoint;

        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoWrong), _cancellationToken);
        }

        // Act — attempt with correct password while locked
        var loginDtoCorrect = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = Utils.TestUserPassword
        };

        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoCorrect), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync(_cancellationToken);
        body.Should().Contain("temporarily locked");
    }

    [Fact]
    public async Task Authenticate_ReturnsOk_WhenLoginAfterLockoutExpires()
    {
        // Arrange — create test user and trigger lockout
        await Utils.InsertTestUser(_client);

        var loginDtoWrong = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = string.Concat(Utils.TestUserPassword, ".")
        };

        var url = Constants.AddressAuthenticationEndpoint;

        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoWrong), _cancellationToken);
        }

        // Wait for the 1-minute lockout configured in appsettings.IntegrationTests.json to expire.
        // A small buffer is added to avoid flakiness from clock drift between when LockoutEnd was set and now.
        await Task.Delay(TimeSpan.FromSeconds(65), _cancellationToken);

        // Act — login with correct credentials after lockout has expired
        var loginDtoCorrect = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = Utils.TestUserPassword
        };

        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoCorrect), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("", "12345678")]
    [InlineData("username", "")]
    public async Task Authenticate_ReturnsBadRequest_WhenMissingUsernameOrPassword(string username, string password)
    {
        // Arrange
        var loginDto = new LoginRequest
        {
            Username = username,
            Password = password
        };

        // Act
        var response = await _client.PostAsync(Constants.AddressAuthenticationEndpoint, Utils.GetJsonContentFromObject(loginDto), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    #endregion
    
    #region Refresh tests
    
    [Fact]
    public async Task Refresh_ReturnsNewLoginResponse_WhenRefreshTokenIsValid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);
        var loginResponse = await Utils.LoginAsync(_client);

        var refreshRequest = new RefreshRequest { RefreshToken = loginResponse.RefreshToken };

        // Act
        var response = await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(refreshRequest), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var newLoginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(_cancellationToken);
        newLoginResponse.Should().NotBeNull();
        newLoginResponse!.AccessToken.Should().NotBeNullOrEmpty();
        newLoginResponse.RefreshToken.Should().NotBeNullOrEmpty();
        newLoginResponse.RefreshToken.Should().NotBe(loginResponse.RefreshToken);
        newLoginResponse.AccessTokenExpiresAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(6));
        newLoginResponse.RefreshTokenExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Refresh_ReturnsUnauthorized_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "invalid-token" };

        // Act
        var response = await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(refreshRequest), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ReturnsUnauthorizedAndRevokeAllTokens_WhenRefreshTokenIsUsedTwice()
    {
        // Arrange
        await Utils.InsertTestUser(_client);
        var loginResponse = await Utils.LoginAsync(_client);
        var secondLoginResponse = await Utils.LoginAsync(_client);

        var refreshRequest = new RefreshRequest { RefreshToken = loginResponse.RefreshToken };
        var secondRefreshRequest = new RefreshRequest { RefreshToken = secondLoginResponse.RefreshToken };

        // Use the refresh token once (rotates it)
        await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(refreshRequest), _cancellationToken);

        // Act
        
        // Reuse the same (now revoked) refresh token
        var response = await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(refreshRequest), _cancellationToken);

        // Tries to use other token
        var secondResponse = await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(secondRefreshRequest), _cancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ReturnsBadRequest_WhenRefreshTokenIsEmpty()
    {
        // Arrange
        var refreshRequest = new RefreshRequest { RefreshToken = "" };

        // Act
        var response = await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(refreshRequest), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Refresh_ReturnsBadRequest_WhenRefreshTokenIsMissing()
    {
        // Act
        var response = await _client.PostAsync(Constants.AddressRefreshTokenEndpoint, Utils.GetJsonContentFromObject(string.Empty), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
    
}
