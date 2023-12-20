using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MyBuyingList.Web.Tests.IntegrationTests;

/// <summary>
/// TODO para os de integracao
/// GetAllUsersAsync:
///     [INTEGRATION] * Repositório tem mais de 1 página e retorna a 2
///  CreateAsync:
///     [INTEGRATION] Conferir valores (inclusive senha)
/// 
/// [INTEGRATION] Testar operações canceladas?
/// [INTEGRATION] Testar exception no database?
/// </summary>
public class UserControllerIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    public UserControllerIntegrationTests(ResourceFactory webAppFactory)
        : base(webAppFactory)
    {
        _client = webAppFactory.HttpClient;
    }

    private async Task<string> GetJwtToken()
    {
        var response = await _client.GetAsync("api/auth?username=admin&password=123");
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }

    [Fact]
    public async void GetAllUsersAsync_ShouldReturnGetUserDtoList_WhenThereAreUsers()
    {
        // Arrange
        var expectedUsers = new List<GetUserDto>() {
            new GetUserDto { Id = 1, UserName = "admin", Email = "marcelluscfarias@gmail.com", Active = true,   }
        };

        // Act
        var token = await this.GetJwtToken();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("api/user");

        // Assert
        var users = await response.Content.ReadFromJsonAsync<List<GetUserDto>>()!;
        users.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async void CreateUserAsync_ShouldReturnCreated_WhenDtoIsGood()
    {
        //Arrange
        var newUser = new CreateUserDto
        {
            Email = "newemail@gmail.com",
            Password = "Pa12345678!",
            UserName = "user2"
        };

        var jsonBody = JsonSerializer.Serialize(newUser);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        //Act
        var token = await this.GetJwtToken();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync("api/user", httpContent);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }
}