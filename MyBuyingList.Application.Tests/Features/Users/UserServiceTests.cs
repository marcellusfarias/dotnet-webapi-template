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
    private readonly UserService _sut;
    private readonly IFixture _fixture;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IPasswordEncryptionService _passwordEncryptionService = Substitute.For<IPasswordEncryptionService>();

    private const int DefaultPage = 1;

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
    public async Task GetAllUsersAsync_ShouldReturnGetUserDtoList_WhenThereAreUsers()
    {
        //Arrange
        User user1 = _fixture.Create<User>();
        User user2 = _fixture.Create<User>();
        User user3 = _fixture.Create<User>();

        List<User> mockedUsers = [user1, user2, user3];
        _userRepositoryMock
            .GetAllAsync(DefaultPage, CancellationToken.None)
            .Returns(mockedUsers);

        //Act
        var returnUsers = await _sut.GetAllUsersAsync(DefaultPage, CancellationToken.None);

        //Assert
        GetUserDto userDto1 = new GetUserDto(user1.Id, user1.UserName, user1.Email, user1.Active);
        GetUserDto userDto2 = new GetUserDto(user2.Id, user2.UserName, user2.Email, user2.Active);
        GetUserDto userDto3 = new GetUserDto(user3.Id, user3.UserName, user3.Email, user3.Active);
        var expectedUserDtos = new List<GetUserDto>() { userDto1, userDto2, userDto3 };

        returnUsers.Should().BeEquivalentTo(expectedUserDtos);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenThereAreNoUsers()
    {
        //Arrange
        _userRepositoryMock
            .GetAllAsync(DefaultPage, CancellationToken.None)
            .Returns([]);

        //Act
        var users = await _sut.GetAllUsersAsync(DefaultPage, CancellationToken.None);

        //Assert
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserAsync_ShouldReturnUser_WhenIdExists()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int searchingUserId = user.Id;

        _userRepositoryMock
            .GetAsync(searchingUserId, CancellationToken.None)
            .Returns(user);

        //Act
        var returnUser = await _sut.GetUserAsync(searchingUserId, CancellationToken.None);

        //Assert
        GetUserDto userDto2 = new GetUserDto(searchingUserId, user.UserName, user.Email, user.Active);

        returnUser.Should().BeEquivalentTo(userDto2);
    }

    [Fact]
    public async Task GetUserAsync_ShouldThrowResourceNotFoundException_WhenIdDoesNotExists()
    {
        //Arrange
        int randomId = _fixture.Create<int>();

        _userRepositoryMock
            .GetAsync(randomId, CancellationToken.None)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.GetUserAsync(randomId, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnId_WhenDtoIsValid()
    {
        // Arrange
        var randomId = _fixture.Create<int>();
        var createUser = _fixture.Create<CreateUserDto>();
        _userRepositoryMock
            .AddAsync(Arg.Any<User>(), CancellationToken.None)
            .Returns(randomId);

        // Act
        var createdId = await _sut.CreateAsync(createUser, CancellationToken.None);

        // Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning �Consider applying the 'await' operator�
        _userRepositoryMock.Received(1).AddAsync(Arg.Any<User>(), CancellationToken.None);
#pragma warning restore 4014

        createdId.Should().Be(randomId);

    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldReturnVoid_WhenSucceded()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        string oldPassoword = "12345678";
        string newPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetAsync(userId, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(oldPassoword, user.Password)
            .Returns(true);

        //Act
        await _sut.ChangeUserPasswordAsync(userId, oldPassoword, newPassword, CancellationToken.None);

        //Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning �Consider applying the 'await' operator�
        _userRepositoryMock.Received(1).EditAsync(Arg.Any<User>(), CancellationToken.None);
#pragma warning restore 4014
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldThrowBusinessLogicException_WhenPasswordsDontMatch()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        string oldPassoword = "12345678";
        string newPassword = _fixture.Create<string>();

        _userRepositoryMock
            .GetAsync(userId, CancellationToken.None)
            .Returns(user);

        _passwordEncryptionService
            .VerifyPasswordsAreEqual(oldPassoword, user.Password)
            .Returns(false);

        //Act
        var act = async () => await _sut.ChangeUserPasswordAsync(userId, oldPassoword, newPassword, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BusinessLogicException>();
    }

    [Fact]
    public async Task ChangeUserPasswordAsync_ShouldThrowResourceNotFoundException_WhenUserDoesNotExist()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;

        _userRepositoryMock
            .GetAsync(userId, CancellationToken.None)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.ChangeUserPasswordAsync(userId, "", "", CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnVoid_WhenSucceded()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;

        _userRepositoryMock
            .GetAsync(userId, CancellationToken.None)
            .Returns(user);

        //Act
        await _sut.DeleteAsync(userId, CancellationToken.None);

        //Assert
#pragma warning disable 4014 //for .Received await is not required, so suppress warning �Consider applying the 'await' operator�
        _userRepositoryMock.Received(1).LogicalExclusionAsync(Arg.Any<User>(), CancellationToken.None);
#pragma warning restore 4014
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowResourceNotFoundException_WhenUserDoesNotExist()
    {
        //Arrange
        int userId = _fixture.Create<int>();

        _userRepositoryMock
            .GetAsync(userId, CancellationToken.None)
            .ReturnsNull();

        //Act
        var act = async () => await _sut.DeleteAsync(userId, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowBusinessLogicException_WhenUserIsAdmin()
    {
        //Arrange
        var user = _fixture.Create<User>();
        int userId = user.Id;
        user.UserName = "admin";

        _userRepositoryMock
            .GetAsync(userId, CancellationToken.None)
            .Returns(user);

        //Act
        var act = async () => await _sut.DeleteAsync(userId, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BusinessLogicException>();
    }
}