using Microsoft.Extensions.Configuration;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace MyBuyingList.Web.Tests.IntegrationTests;

/// <summary>
/// [INTEGRATION] Testar operações canceladas?
/// [INTEGRATION] Testar exception no database?
/// </summary>
public class UserControllerIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly int _pageSize;
    private readonly string _validPassword = "Pa12345678!";

    public UserControllerIntegrationTests(ResourceFactory resourceFactory)
        : base(resourceFactory)
    {
        _client = resourceFactory.HttpClient;
        
        _pageSize = resourceFactory.Configuration.GetValue<int>("RepositorySettings:PageSize");

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // https://stackoverflow.com/questions/49160306/why-a-customization-returns-the-same-instance-when-applied-on-two-different-prop
        _fixture.Customize<CreateUserDto>(c =>
            c.With(x => x.Password, _validPassword)
                .Without(x => x.Email)
                .Do(x =>
                {
                    x.Email = _fixture.Create<MailAddress>().Address;
                }));
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

        if(response.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Could not create test user.");
        }
    }


    [Fact]
    public async void GetAllUsersAsync_ShouldReturnGetUserDtoList_WhenThereAreUsers()
    {
        // Arrange
        var expectedUsers = new List<GetUserDto>() {
            new GetUserDto { Id = 1, UserName = "admin", Email = "marcelluscfarias@gmail.com", Active = true,   }
        };

        // Act
        var response = await _client.GetAsync("api/users");

        // Assert
        var users = await response.Content.ReadFromJsonAsync<List<GetUserDto>>()!;
        users.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async void GetAllUsersAsync_ShouldReturnUnauthorized_WhenNoTokenIsProvided()
    {
        // Arrange
        var auth = _client.DefaultRequestHeaders.Authorization;
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("api/users");
        _client.DefaultRequestHeaders.Authorization = auth;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async void GetAllUsersAsync_ShouldReturnSecondPage_WhenPageTwoIsAsked()
    {
        // Arrange
        var extraSize = 5;
        var createUsers = _fixture.CreateMany<CreateUserDto>(_pageSize + extraSize).ToList();
        var tasks = new List<Task>();

        foreach (var user in createUsers)
        {
            user.UserName = user.UserName.Substring(32);

            var jsonBody = JsonSerializer.Serialize(user);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            tasks.Add(Task.Run(() => _client.PostAsync("api/user", httpContent)));
        }

        await Task.WhenAll(tasks);

        // Act
        var response = await _client.GetAsync("api/users?page=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<GetUserDto>>()!;
        users.Should().HaveCount(extraSize + 1); // db already has user ADMIN
    }

    // TODO: not sure how to force an internal server error.
    // Should create another Factory?
    //[Fact]
    //public async void GetAllUsersAsync_ShouldReturnInternalServerError_WhenDatabaseIsDown()
    //{
    //}

    [Fact]
    public async void GetUserById_ShouldReturnUser_WhenIdExists()
    {
        // Arrange
        var expectedUser = new GetUserDto { Id = 1, UserName = "admin", Email = "marcelluscfarias@gmail.com", Active = true, };

        // Act
        var response = await _client.GetAsync("api/user/1");

        // Assert
        var users = await response.Content.ReadFromJsonAsync<List<GetUserDto>>()!;
        users.Should().BeEquivalentTo(new List<GetUserDto>() { expectedUser });
    }

    [Fact]
    public async void GetUserById_ShouldReturnNotFoound_WhenIdDoesntExist()
    {
        // Act
        var response = await _client.GetAsync("api/user/100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void CreateUserAsync_ShouldReturnCreated_WhenDtoIsGood()
    {
        // Arrange
        var newUser = new CreateUserDto
        {
            Email = "newemail@gmail.com",
            Password = _validPassword,
            UserName = "user2"
        };

        var jsonBody = JsonSerializer.Serialize(newUser);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("api/user", httpContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadFromJsonAsync<CreateUserDto>();
        responseContent.Should().BeEquivalentTo(newUser);
    }

    [Fact]
    public async void CreateUserAsync_ShouldReturnBadRequest_WhenDtoIsNotOk()
    {
        // Arrange
        var newUser = new CreateUserDto
        {
            Email = "newemail@com",
            Password = ".",
            UserName = "user2"
        };

        var jsonBody = JsonSerializer.Serialize(newUser);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("api/user", httpContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadFromJsonAsync<ErrorModel>();
        responseContent!.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async void DeleteUserAsync_ShouldReturnUnprocessableEntity_WhenIdIsAdmin()
    {
        // Act
        var response = await _client.DeleteAsync("api/user/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        // test body
    }

    [Fact]
    public async void DeleteUserAsync_ShouldReturnNoContent_WhenIdExists()
    {
        // Arrange
        await InsertTestUser();

        // Act
        var response = await _client.DeleteAsync("api/user/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async void DeleteUserAsync_ShouldReturnNotFound_WhenIdDoesntExist()
    {
        // Act
        var response = await _client.DeleteAsync("api/user/100");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void ChangePassword_ShouldReturnNoContent_WhenDtoIsOk()
    {
        // Arrange
        await InsertTestUser();

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
        {
            OldPassword = _validPassword,
            NewPassword = "Mn!90..pT",
        };

        var jsonBody = JsonSerializer.Serialize(dto);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("api/user/2/password", httpContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }

    [Fact]
    public async void ChangePassword_ShouldReturnBadRequest_WhenDtoIsNotOk()
    {
        // Arrange
        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
        {
            OldPassword = _validPassword,
            NewPassword = ".",
        };

        var jsonBody = JsonSerializer.Serialize(dto);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("api/user/2/password", httpContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadFromJsonAsync<ErrorModel>();
        responseContent!.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async void ChangePassword_ShouldReturnNotFound_WhenIdDoesntExist()
    {
        // Arrange
        await InsertTestUser();

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
        {
            OldPassword = _validPassword,
            NewPassword = "Mn!90..pT",
        };

        var jsonBody = JsonSerializer.Serialize(dto);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("api/user/200/password", httpContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void ChangePassword_ShouldReturnUnprocessableEntity_WhenOldPasswordDoesntMatchCurrentOne()
    {
        // Arrange
        await InsertTestUser();

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
        {
            OldPassword = string.Concat(_validPassword, "."),
            NewPassword = "Mn!90..pT",
        };

        var jsonBody = JsonSerializer.Serialize(dto);
        var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("api/user/2/password", httpContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}