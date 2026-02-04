using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;

namespace MyBuyingList.Web.Tests.IntegrationTests;

// TODO: test RateLimiting
public class AuthControllerIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    
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
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
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
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        var response = await _client.PostAsync(url, Utils.GetJsonContentFromObject(loginDto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
