using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MyBuyingList.Domain.Entities;
using FluentAssertions;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Application.Tests.UnitTests;

public class CustomAuthenticationServiceTests
{
    private CustomAuthenticationService _sut;

    private readonly Mock<IJwtProvider> _jwtProviderMock = new Mock<IJwtProvider>();
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();

    public CustomAuthenticationServiceTests()
    {
        _sut = new CustomAuthenticationService(_userRepositoryMock.Object, _jwtProviderMock.Object);
    }

    [Fact]
    public void AuthenticateAndReturnToken_WhenUserDoesNotExist_ShouldThrowException()
    {
        //Arrange
        string username = "testingUser";
        string password = "123";

        _userRepositoryMock
            .Setup(x => x.GetAuthenticationDataByUsername(username))
            .Returns((User?) null);

        //Act
        var action = _sut.Invoking(x => _sut.AuthenticateAndReturnToken(username, password));

        //Assert
        var errorMessage = "An error occured when authenticating user testingUser.";
        action.Should()
            .Throw<AuthenticationException>()
            .WithMessage(errorMessage);
    }

    [Fact]
    public void AuthenticateAndReturnToken_WhenWrongPassword_ShouldThrowException()
    {
        //Arrange
        string username = "testingUser";
        string password = "123";

        User user = new User() 
        { 
            Id = 1,            
            UserName = "testingUser",
            Password = "wrongpassword",
            Email = "anything@gmail.com",
            Active = true,
        };

        _userRepositoryMock
            .Setup(x => x.GetAuthenticationDataByUsername(username))
            .Returns(user);

        //Act
        var action = _sut.Invoking(x => _sut.AuthenticateAndReturnToken(username, password));

        //Assert
        var errorMessage = "An error occured when authenticating user testingUser.";
        action.Should()
            .Throw<AuthenticationException>()
            .WithMessage(errorMessage);
    }

    [Fact]
    public void AuthenticateAndReturnToken_WhenAuthenticated_ShouldReturnToken()
    {
        //Arrange
        string username = "testingUser";
        string password = "123";

        User user = new User()
        {
            Id = 1,
            UserName = "testingUser",
            Password = "123",
            Email = "anything@gmail.com",
            Active = true,
        };

        _userRepositoryMock
            .Setup(x => x.GetAuthenticationDataByUsername(username))
            .Returns(user);

        _jwtProviderMock
            .Setup(x => x.Generate(user.Email))
            .Returns("mockedJwtToken");

        //Act
        var token = _sut.AuthenticateAndReturnToken(username, password);

        //Assert
        token.Should().BeEquivalentTo("mockedJwtToken");
    }

}
