using FluentAssertions;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Features.Login.Services;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Domain.Entities;
using NSubstitute;

namespace MyBuyingList.Application.Tests.UnitTests;

public class LoginServiceTests
{
    private LoginService _sut;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IJwtProvider _jwtProviderMock = Substitute.For<IJwtProvider>();
    private readonly IPasswordEncryptionService _passwordEncryptionService = Substitute.For<IPasswordEncryptionService>();
    public LoginServiceTests()
    {
        _sut = new LoginService(_userRepositoryMock, _jwtProviderMock, _passwordEncryptionService);
    }

    public static IEnumerable<User> MockerUsers =>
        new List<User>()
        {
            new User()
            {
                Id = 1,
                Email = "marcellus@hotmail.com",
                UserName = "marcellus",
                Password = "123",
                CreatedAt = DateTime.Now,
                Active = true,
            },
            new User()
            {
                Id =2,
                Email = "raffael@hotmail.com",
                UserName = "raffael",
                Password = "321",
                CreatedAt = DateTime.Now,
                Active = true,
            }
        };

    [Fact]
    public async void AuthenticateAndReturnJwtToken_ReturnsToken_WhenAuthenticationIsValid()
    {
        //Arrange
        var user = new User()
        {
            Id = 1,
            Email = "marcellus@hotmail.com",
            UserName = "marcellus",
            Password = "123",
            CreatedAt = DateTime.Now,
            Active = true,
        };

        _userRepositoryMock
            .GetAllAsync(1, default)
            .Returns(MockerUsers.ToList());

        _jwtProviderMock
            .GenerateTokenAsync(user.Id, default)
            .Returns("custom_token");

        //Act
        string token = await _sut.AuthenticateAndReturnJwtTokenAsync(user.UserName, user.Password, default);

        //Assert
        token.Should().BeEquivalentTo("custom_token");
    }

    //[Fact]
    //public async void AuthenticateAndReturnJwtToken_WhenUserDoesNotExist_ShouldThrowException()
    //{
    //    //Arrange
    //    var user = new User()
    //    {
    //        Id = 3,
    //        Email = "bob@hotmail.com",
    //        UserName = "bob",
    //        Password = "123",
    //        CreatedAt = DateTime.Now,
    //        Active = true,
    //    };

    //    _userRepositoryMock
    //        .Setup(x => x.GetAllAsync().Result)
    //        .Returns(MockerUsers);

    //    //Act
    //    var action =  _sut.Invoking(x => _sut.AuthenticateAndReturnJwtTokenAsync(user.UserName, user.Password));
        
    //    // Assert
    //    action.Should()
    //        .Throw<AuthenticationException>()
    //        .WithMessage("An error occured when authenticating user bob.");
    //}

    //[Fact]
    //public void AuthenticateAndReturnJwtToken_WhenWrongPassword_ShouldThrowException()
    //{
    //    //Arrange
    //    var user = new User()
    //    {
    //        Id = 1,
    //        Email = "marcellus@hotmail.com",
    //        UserName = "marcellus",
    //        Password = "wrong_password",
    //        CreatedAt = DateTime.Now,
    //        Active = true,
    //    };

    //    _userRepositoryMock
    //        .Setup(x => x.GetAllAsync().Result)
    //        .Returns(MockerUsers);

    //    //Act
    //    var action = _sut.Invoking(x => _sut.AuthenticateAndReturnJwtToken(user.UserName, user.Password));

    //    //Assert
    //    action.Should()
    //       .Throw<AuthenticationException>()
    //       .WithMessage("An error occured when authenticating user marcellus.");

    //}
}
