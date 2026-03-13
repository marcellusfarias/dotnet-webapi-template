using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;
using System.Text.Json;

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

    [Fact]
    public async Task Authenticate_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        var loginDto = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = Utils.TestUserPassword
        };

        // Act
        var url = Constants.AddressAuthenticationEndpoint;
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDto), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync(_cancellationToken);
        responseContent.Should().NotBeNull();
    }

    [Fact]
    public async Task Authenticate_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        var invalidPassword = string.Concat(Utils.TestUserPassword, ".");

        var loginDto = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = invalidPassword
        };

        // Act        
        var url = Constants.AddressAuthenticationEndpoint;
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDto), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Authenticate_ReturnsUnauthorized_WithLockoutMessage_WhenAccountIsLocked()
    {
        // Arrange — create a test user and exhaust MaxFailedAttempts (3 in test config)
        await Utils.InsertTestUser(_client);

        var invalidPassword = string.Concat(Utils.TestUserPassword, ".");
        var loginDtoWrong = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = invalidPassword
        };

        var url = Constants.AddressAuthenticationEndpoint;

        // Send MaxFailedAttempts wrong attempts to trigger lockout
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

        var invalidPassword = string.Concat(Utils.TestUserPassword, ".");
        var loginDtoWrong = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = invalidPassword
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

        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDtoCorrect),  _cancellationToken);

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

        var invalidPassword = string.Concat(Utils.TestUserPassword, ".");
        var loginDtoWrong = new LoginRequest
        {
            Username = Utils.TestUserUsername,
            Password = invalidPassword
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
        var url = Constants.AddressAuthenticationEndpoint;
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDto), _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
