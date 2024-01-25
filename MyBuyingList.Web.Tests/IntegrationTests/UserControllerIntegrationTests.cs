using Microsoft.Extensions.Configuration;
using MyBuyingList.Application.Common.Constants;
using MyBuyingList.Application.Features.Login.DTOs;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;

namespace MyBuyingList.Web.Tests.IntegrationTests;

// TODO: test cancelled operations & test database exceptions
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
        //_fixture.Customize<CreateUserDto>(customization =>
        //    customization
        //        .With(x => x.Password, _validPassword)
        //        .Without(x => x.UserName)
        //        .Do(x =>
        //        {
        //            x.UserName = _fixture.Create<string>().Substring(32);
        //        })
        //        .Without(x => x.Email)
        //        .Do(x =>
        //        {
        //            x.Email = _fixture.Create<MailAddress>().Address;
        //        }));

        _fixture.Customize<CreateUserDto>(customization =>
            customization
                .With(x => x.Password, _validPassword)
                .Without(x => x.UserName)
                .Without(x => x.Email)
                .Do(x =>
                {
                    var username = _fixture.Create<string>().Substring(32);
                    var email = _fixture.Create<MailAddress>().Address;
                    x = x with { UserName = username, Email = email };
                })
        );
    }

    [Fact]
    public async void GetAllUsersAsync_ShouldReturnGetUserDtoList_WhenThereAreUsers()
    {
        // Arrange
        var adminUser = MyBuyingList.Domain.Constants.Users.AdminUser;
        var expectedUser = new List<GetUserDto>()
        {
            new GetUserDto(adminUser.Id, adminUser.UserName, adminUser.Email, true)
        };

        // Act
        var response = await _client.GetAsync("api/users");

        // Assert
        var users = await response.Content.ReadFromJsonAsync<List<GetUserDto>>()!;
        users.Should().BeEquivalentTo(expectedUser);
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
    public async void GetAllUsersAsync_ShouldReturnForbidden_WhenUserWithoutPermissionsRequests()
    {
        // Arrange
        await Utils.InsertTestUser(_client);

        LoginDto dto = new() 
        {
            Username = Utils.TESTUSER_USERNAME,
            Password = Utils.TESTUSER_PASSWORD
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
    public async void GetAllUsersAsync_ShouldReturnSecondPage_WhenPageTwoIsAsked()
    {
        // Arrange
        var extraSize = 5;
        var createUsers = _fixture.CreateMany<CreateUserDto>(_pageSize + extraSize).ToList();
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
        var users = await response.Content.ReadFromJsonAsync<List<GetUserDto>>()!;
        users.Should().HaveCount(extraSize + 1); // db already has user ADMIN
    }

    [Fact]
    public async void GetUserById_ShouldReturnUser_WhenIdExists()
    {
        // Arrange
        var adminUser = MyBuyingList.Domain.Constants.Users.AdminUser;
        var expectedUser = new GetUserDto(adminUser.Id, adminUser.UserName, adminUser.Email, true);

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, adminUser.Id);
        var response = await _client.GetAsync(url);

        // Assert
        var user = await response.Content.ReadFromJsonAsync<GetUserDto>()!;
        user.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async void GetUserById_ShouldReturnNotFoound_WhenIdDoesntExist()
    {
        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 100);
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void CreateUserAsync_ShouldReturnCreated_WhenDtoIsGood()
    {
        // Arrange
        var newUser = _fixture.Create<CreateUserDto>();

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
    public async void CreateUserAsync_ShouldReturnBadRequest_WhenDtoIsNotOk()
    {
        // Arrange
        var newUser = _fixture.Create<CreateUserDto>();
        newUser = newUser with
        {
            Email = "bademail@com",
            Password = "."
        };

        var expectedErrorModel = new ErrorModel()
        {
            Errors = new List<ErrorDetails>()
            {
                new ErrorDetails(){ Title = "Error validating property 'Email'.", Detail = ValidationMessages.INVALID_EMAIL },
                new ErrorDetails(){ Title = "Error validating property 'Password'.", Detail = ValidationMessages.INVALID_PASSWORD },
            }
        };

        // Act
        var response = await _client.PostAsync(
            requestUri: Constants.BaseAddressUserEndpoint, 
            content: Utils.GetJsonContentFromObject(newUser)
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseErrorModel = await response.Content.ReadFromJsonAsync<ErrorModel>();
        responseErrorModel!.Should().BeEquivalentTo(expectedErrorModel);
    }

    [Fact]
    public async void DeleteUserAsync_ShouldReturnUnprocessableEntity_WhenIdIsAdmin()
    {
        // Arrange
        var expectedErrorModel = ErrorModel.CreateSingleErrorDetailsModel(
            ErrorMessages.BusinessLogicError, 
            "Can't disable user admin.");

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 1);
        var response = await _client.DeleteAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var content = await response.Content.ReadFromJsonAsync<ErrorModel>();
        content!.Should().BeEquivalentTo(expectedErrorModel);
    }

    [Fact]
    public async void DeleteUserAsync_ShouldReturnNoContent_WhenIdExists()
    {
        // Arrange
        var createdId = await Utils.InsertTestUser(_client);

        // Act
        var response = await _client.DeleteAsync(string.Concat(Constants.BaseAddressUserEndpoint, createdId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getUserResponse = await _client.GetAsync(string.Concat(Constants.BaseAddressUserEndpoint, createdId));
        getUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await getUserResponse.Content.ReadFromJsonAsync<GetUserDto>();
        content!.Active.Should().Be(false);
    }

    [Fact]
    public async void DeleteUserAsync_ShouldReturnNotFound_WhenIdDoesntExist()
    {
        // Act
        var response = await _client.DeleteAsync(string.Concat(Constants.BaseAddressUserEndpoint, 100));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async void ChangePassword_ShouldReturnNoContent_WhenDtoIsOk()
    {
        // Arrange
        var createdId = await Utils.InsertTestUser(_client);

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto(Utils.TESTUSER_PASSWORD, "Mn!90..pT");

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, createdId, "/password");
        var response = await _client.PutAsync(url, Utils.GetJsonContentFromObject(dto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // If this operation is successfull, 
        // we can infer previous operation really updated the password
        // in the database
        dto = new UpdateUserPasswordDto("Mn!90..pT", Utils.TESTUSER_PASSWORD);
        response = await _client.PutAsync(url, Utils.GetJsonContentFromObject(dto));
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async void ChangePassword_ShouldReturnBadRequest_WhenDtoIsNotOk()
    {
        // Arrange
        UpdateUserPasswordDto dto = new UpdateUserPasswordDto(_validPassword, ".");
        
        var expectedErrorModel = ErrorModel.CreateSingleErrorDetailsModel(
            "Error validating property 'NewPassword'.", 
            ValidationMessages.INVALID_PASSWORD);

        // Act
        var url = string.Concat(Constants.BaseAddressUserEndpoint, 2, "/password");
        var response = await _client.PutAsync(url, Utils.GetJsonContentFromObject(dto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadFromJsonAsync<ErrorModel>();
        responseContent!.Should().BeEquivalentTo(expectedErrorModel);
    }

    [Fact]
    public async void ChangePassword_ShouldReturnNotFound_WhenIdDoesntExist()
    {
        // Arrange
        UpdateUserPasswordDto dto = new UpdateUserPasswordDto(_validPassword, "Mn!90..pT");

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
    public async void ChangePassword_ShouldReturnUnprocessableEntity_WhenOldPasswordDoesntMatchCurrentOne()
    {
        // Arrange
        var createdId = await Utils.InsertTestUser(_client);

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto(string.Concat(_validPassword, "."), "Mn!90..pT");

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