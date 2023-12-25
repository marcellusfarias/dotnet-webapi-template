using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MyBuyingList.Web.Tests.IntegrationTests;

public class AuthControllerIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly string _validPassword = "Pa12345678!";

    public AuthControllerIntegrationTests(ResourceFactory resourceFactory)
        : base(resourceFactory)
    {
        _client = resourceFactory.HttpClient;

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    private async Task InsertTestUser()
    {
        var newUser = new CreateUserDto
        {
            Email = "newemail@gmail.com",
            Password = _validPassword,
            UserName = "user2"
        };

        var jsonBody = JsonSerializer.Serialize(newUser);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("api/user", httpContent);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Could not create test user.");
        }
    }

    [Fact]
    public async void Authenticate_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        await InsertTestUser();

        // Act
        var response = await _client.GetAsync($"api/auth?username=user2&password={_validPassword}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNull();
    }

    [Fact]
    public async void Authenticate_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        await InsertTestUser();

        // Act
        var response = await _client.GetAsync($"api/auth?username=user2&password={string.Concat(_validPassword, ".")}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
