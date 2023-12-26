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
    public async void Authenticate_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        // Act
        var url = string.Format(Constants.AddressAuthenticationEndpoint, Utils.TESTUSER_USERNAME, Utils.TESTUSER_PASSWORD);
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNull();
    }

    [Fact]
    public async void Authenticate_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        // Act
        var invalidPassword = string.Concat(Utils.TESTUSER_PASSWORD, ".");
        var url = string.Format(Constants.AddressAuthenticationEndpoint, Utils.TESTUSER_USERNAME, invalidPassword);
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
