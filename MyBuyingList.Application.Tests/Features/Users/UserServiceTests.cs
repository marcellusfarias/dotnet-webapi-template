using MyBuyingList.Application.Features.Users.Services;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Interfaces;
using System.Net.Mail;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Application.Tests.Features.Users;

public class UserServiceTests
{
    private UserService _sut;
    private IFixture _fixture;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IPasswordEncryptionService _passwordEncryptionService = Substitute.For<IPasswordEncryptionService>();

    private const int DEFAULT_PAGE = 1;

    public UserServiceTests()
    {
        _sut = new UserService(_userRepositoryMock, _passwordEncryptionService);
        _fixture = new Fixture();//.Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<User>(c => c
            .With(x =>
                x.Email,
                _fixture.Create<MailAddress>().Address));

        _fixture.Customize<CreateUserDto>(c => c
            .With(x =>
                x.Email,
                _fixture.Create<MailAddress>().Address));
    }

    [Fact]
    public async void GetAllUsersAsync_ShouldReturnGetUserDtoList_WhenThereAreUsers()
    {
        //Arrange
        User user1 = _fixture.Create<User>();
        User user2 = _fixture.Create<User>();
        User user3 = _fixture.Create<User>();

        List<User> mockedUsers = new List<User>() { user1, user2, user3 };
        _userRepositoryMock
            .GetAllAsync(DEFAULT_PAGE, default)
            .Returns(mockedUsers);

        //Act
        var returnUsers = await _sut.GetAllUsersAsync(DEFAULT_PAGE, default);

        //Assert
        GetUserDto userDto1 = new GetUserDto() { Id = user1.Id, UserName = user1.UserName, Email = user1.Email, Active = user1.Active };
        GetUserDto userDto2 = new GetUserDto() { Id = user2.Id, UserName = user2.UserName, Email = user2.Email, Active = user2.Active };
        GetUserDto userDto3 = new GetUserDto() { Id = user3.Id, UserName = user3.UserName, Email = user3.Email, Active = user3.Active };
        var expectedUserDtos = new List<GetUserDto>() { userDto1, userDto2, userDto3 };

        returnUsers.Should().BeEquivalentTo(expectedUserDtos);
    }

    [Fact]
    public async void GetAllUsersAsync_ShouldReturnEmptyList_WhenThereAreNoUsers()
    {
        //Arrange
        _userRepositoryMock
            .GetAllAsync(DEFAULT_PAGE, default)
            .Returns(new List<User>());

        //Act
        var users = await _sut.GetAllUsersAsync(DEFAULT_PAGE, default);

        //Assert
        users.Should().BeEmpty();
    }

    [Fact]
    public async void GetUserAsync_ShouldReturnUser_WhenIdExists()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int searchingUserId = user.Id;

        _userRepositoryMock
            .GetAsync(searchingUserId, default)
            .Returns(user);

        //Act
        var returnUser = await _sut.GetUserAsync(searchingUserId, default);

        //Assert
        GetUserDto userDto2 = new GetUserDto()
        {
            Id = searchingUserId,
            UserName = user.UserName,
            Email = user.Email,
            Active = user.Active
        };

        returnUser.Should().BeEquivalentTo(userDto2);
    }

    [Fact]
    public async void GetUserAsync_ShouldThrowResourceNotFoundException_WhenIdDoesNotExists()
    {
        //Arrange
        int randomId = _fixture.Create<int>();

        _userRepositoryMock
            .GetAsync(randomId, default)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.GetUserAsync(randomId, default);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async void CreateAsync_ShouldReturnId_WhenDtoIsValid()
    {
        // Arrange
        var randomId = _fixture.Create<int>();
        var createUser = _fixture.Create<CreateUserDto>();
        _userRepositoryMock
            .AddAsync(Arg.Any<User>(), default)
            .Returns(randomId);

        // Act
        var createdId = await _sut.CreateAsync(createUser, default);

        // Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning “Consider applying the 'await' operator”
        _userRepositoryMock.Received(1).AddAsync(Arg.Any<User>(), default);
#pragma warning restore 4014

        createdId.Should().Be(randomId);

    }

    [Fact]
    public async void ChangeActiveStatusAsync_ShouldReturnVoid_WhenSucceded()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        bool activeStatus = !user.Active;

        _userRepositoryMock
            .GetAsync(userId, default)
            .Returns(user);

        //Act
        await _sut.ChangeActiveStatusAsync(userId, activeStatus, default);

        //Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning “Consider applying the 'await' operator”
        _userRepositoryMock.Received(1).EditAsync(Arg.Any<User>(), default);
#pragma warning restore 4014
    }

    [Fact]
    public async void ChangeActiveStatusAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        //Arrange
        int userId = _fixture.Create<int>();

        _userRepositoryMock
            .GetAsync(userId, default)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.ChangeActiveStatusAsync(userId, false, default);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async void ChangeUserPasswordAsync_ShouldReturnVoid_WhenSucceded()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        string oldPassoword = "12345678";
        string newPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetAsync(userId, default)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(oldPassoword, user.Password)
            .Returns(true);

        //Act
        await _sut.ChangeUserPasswordAsync(userId, oldPassoword, newPassword, default);

        //Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning “Consider applying the 'await' operator”
        _userRepositoryMock.Received(1).EditAsync(Arg.Any<User>(), default);
#pragma warning restore 4014
    }

    [Fact]
    public async void ChangeUserPasswordAsync_ShouldThrowBusinessLogicException_WhenPasswordsDontMatch()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        string oldPassoword = "12345678";
        string newPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetAsync(userId, default)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(oldPassoword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.ChangeUserPasswordAsync(userId, oldPassoword, newPassword, default);

        //Assert
        await act.Should().ThrowAsync<BusinessLogicException>();
    }

    [Fact]
    public async void ChangeUserPasswordAsync_ShouldThrowResourceNotFoundException_WhenUserDoesNotExist()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;

        _userRepositoryMock
            .GetAsync(userId, default)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.ChangeUserPasswordAsync(userId, "", "", default);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async void DeleteAsync_ShouldReturnVoid_WhenSucceded()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;

        _userRepositoryMock
            .GetAsync(userId, default)
            .Returns(user);

        //Act
        await _sut.DeleteAsync(userId, default);

        //Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning “Consider applying the 'await' operator”
        _userRepositoryMock.Received(1).LogicalExclusionAsync(Arg.Any<User>(), default);
#pragma warning restore 4014
    }

    [Fact]
    public async void DeleteAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        //Arrange
        int userId = _fixture.Create<int>();

        _userRepositoryMock
            .GetAsync(userId, default)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.DeleteAsync(userId, default);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async void DeleteAsync_ShouldThrowException_WhenUserIsAdmin()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        user.UserName = "admin";

        _userRepositoryMock
            .GetAsync(userId, default)
            .Returns(user);

        //Act
        var act = async () => await _sut.DeleteAsync(userId, default);

        //Assert
        await act.Should().ThrowAsync<BusinessLogicException>();
    }
}