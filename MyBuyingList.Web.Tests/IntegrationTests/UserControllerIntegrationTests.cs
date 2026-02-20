using Microsoft.Extensions.Configuration;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Web.Tests.IntegrationTests;

// TODO: test cancelled operations & test database exceptions
public class UserControllerIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly int _pageSize;
    private const string ValidPassword = "Pa12345678!";

    public UserControllerIntegrationTests(ResourceFactory resourceFactory)
        : base(resourceFactory)
    {
        _client = resourceFactory.HttpClient;

        _pageSize = resourceFactory.Configuration.GetValue<int>("RepositorySettings:PageSize");

        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    // I could not manage to use AutoFixture customization with records. See: https://github.com/AutoFixture/AutoFixture/issues/1201
    // I was trying to have the same result as I had for class. Check: // https://stackoverflow.com/questions/49160306/why-a-customization-returns-the-same-instance-when-applied-on-two-different-prop
    private CreateUserRequest GenerateCreateUserRequest()
    {
        var username = _fixture.Create<string>().Substring(32);
        var email = _fixture.Create<MailAddress>().Address;
        var password = ValidPassword;

        return new CreateUserRequest(username, email, password);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUserDtoList_WhenThereAreUsers()
    {
        // Act
        var response = await _client.GetAsync("api/users");

        // Assert
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>()!;
        users.Should().HaveCount(2); // db already has user ADMIN and integration admin
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUnauthorized_WhenNoTokenIsProvided()
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
    public async Task GetAllUsersAsync_ShouldReturnForbidden_WhenUserWithoutPermissionsRequests()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        LoginRequest dto = new() 
        {
            Username = Utils.TestUserUsername,
            Password = Utils.TestUserPassword
        };

        var url = Constants.AddressAuthenticationEndpoint;
        var tokenResponse = await _client.PostAsync(url, Utils.GetJsonContentFromObject(dto));
        var token = await tokenResponse.Content.ReadAsStringAsync();

        var auth = _client.DefaultRequestHeaders.Authorization;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("api/users");
        _client.DefaultRequestHeaders.Authorization = auth;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnSecondPage_WhenPageTwoIsAsked()
    {
        // Arrange
        var extraSize = 5;
        var createUsers = new List<CreateUserRequest>();
        for (int i = 0; i < _pageSize + extraSize; i++)
        {
            createUsers.Add(GenerateCreateUserRequest());
        }

        var tasks = new List<Task>();

        foreach (var user in createUsers)
        {
            tasks.Add(Task.Run(() => _client.PostAsync(
                requestUri: Constants.BaseAddressUserEndpoint, 
                content: Utils.GetJsonContentFromObject(user))
            ));
        }

        await Task.WhenAll(tasks);

        // Act
        var url = "api/users?page=2";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>()!;
        users.Should().HaveCount(extraSize + 2); // db already has user ADMIN and integration admin
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenIdExists()
    {
        // Arrange
        var adminUser = MyBuyingList.Domain.Constants.Users.AdminUser;
        var expectedUser = new UserDto(adminUser.Id, adminUser.UserName, adminUser.Email, true);

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, adminUser.Id);
        var response = await _client.GetAsync(url);

        // Assert
        var user = await response.Content.ReadFromJsonAsync<UserDto>()!;
        user.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFoound_WhenIdDoesntExist()
    {
        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 100);
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnCreated_WhenDtoIsGood()
    {
        // Arrange
        var newUser = GenerateCreateUserRequest();

        // Act
        var response = await _client.PostAsync(
            requestUri: Constants.BaseAddressUserEndpoint, 
            content: Utils.GetJsonContentFromObject(newUser)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdId = await response.Content.ReadFromJsonAsync<int>();

        var getUserResponse = await _client.GetAsync(string.Concat(Constants.BaseAddressUserEndpoint, createdId));
        getUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnBadRequest_WhenDtoIsNotOk()
    {
        // Arrange
        var newUser = GenerateCreateUserRequest();
        newUser = newUser with
        {
            Email = "bademail@com",
            Password = "."
        };

        var expectedErrorResponse = new ErrorResponse()
        {
            Errors =
            [
                new ErrorDetail()
                    { Title = "Error validating property 'Email'.", Detail = ValidationMessages.InvalidEmail },
                new ErrorDetail()
                    { Title = "Error validating property 'Password'.", Detail = ValidationMessages.InvalidPassword }
            ]
        };

        // Act
        var response = await _client.PostAsync(
            requestUri: Constants.BaseAddressUserEndpoint, 
            content: Utils.GetJsonContentFromObject(newUser)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseErrorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        responseErrorResponse!.Should().BeEquivalentTo(expectedErrorResponse);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnUnprocessableEntity_WhenIdIsAdmin()
    {
        // Arrange
        var expectedErrorResponse = ErrorResponse.CreateSingleErrorDetail(
            ErrorMessages.BusinessLogicError, 
            "Can't disable user admin.");

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 1);
        var response = await _client.DeleteAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        content!.Should().BeEquivalentTo(expectedErrorResponse);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnNoContent_WhenIdExists()
    {
        // Arrange
        var createdId = await Utils.InsertTestUser(_client);

        // Act
        var response = await _client.DeleteAsync(string.Concat(Constants.BaseAddressUserEndpoint, createdId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getUserResponse = await _client.GetAsync(string.Concat(Constants.BaseAddressUserEndpoint, createdId));
        getUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await getUserResponse.Content.ReadFromJsonAsync<UserDto>();
        content!.Active.Should().Be(false);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnNotFound_WhenIdDoesntExist()
    {
        // Act
        var response = await _client.DeleteAsync(string.Concat(Constants.BaseAddressUserEndpoint, 100));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnNoContent_WhenDtoIsOk()
    {
        // Arrange
        var createdId = await Utils.InsertTestUser(_client);

        ChangePasswordRequest dto = new ChangePasswordRequest(Utils.TestUserPassword, "Mn!90..pT");

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, createdId, "/password");
        var response = await _client.PutAsync(url, Utils.GetJsonContentFromObject(dto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // If this operation is successfull, 
        // we can infer previous operation really updated the password
        // in the database
        dto = new ChangePasswordRequest("Mn!90..pT", Utils.TestUserPassword);
        response = await _client.PutAsync(url, Utils.GetJsonContentFromObject(dto));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WhenDtoIsNotOk()
    {
        // Arrange
        ChangePasswordRequest dto = new ChangePasswordRequest(ValidPassword, ".");
        
        var expectedErrorResponse = ErrorResponse.CreateSingleErrorDetail(
            "Error validating property 'NewPassword'.", 
            ValidationMessages.InvalidPassword);

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 2, "/password");
        var response = await _client.PutAsync(url, Utils.GetJsonContentFromObject(dto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        responseContent!.Should().BeEquivalentTo(expectedErrorResponse);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnNotFound_WhenIdDoesntExist()
    {
        // Arrange
        ChangePasswordRequest dto = new(ValidPassword, "Mn!90..pT");

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 200, "/password");
        var response = await _client.PutAsync(
            requestUri: url, 
            content: Utils.GetJsonContentFromObject(dto)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnprocessableEntity_WhenOldPasswordDoesntMatchCurrentOne()
    {
        // Arrange
        var createdId = await Utils.InsertTestUser(_client);

        ChangePasswordRequest dto = new(string.Concat(ValidPassword, "."), "Mn!90..pT");

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, createdId, "/password");
        var response = await _client.PutAsync(
            requestUri: url, 
            content: Utils.GetJsonContentFromObject(dto)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}