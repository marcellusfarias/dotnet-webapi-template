﻿using FluentAssertions;
using Moq;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Services;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Application.Tests.UnitTests;

public class LoginServiceTests
{
    private LoginService _sut;
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IJwtProvider> _jwtProviderMock = new Mock<IJwtProvider>();
    public LoginServiceTests()
    {
        _sut = new LoginService(_userRepositoryMock.Object, _jwtProviderMock.Object);
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
    public void AuthenticateAndReturnJwtToken_ReturnsToken_WhenAuthenticationIsValid()
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
            .Setup(x => x.GetAll())
            .Returns(MockerUsers);

        _jwtProviderMock
            .Setup(x => x.GenerateToken(user.Id))
            .Returns("custom_token");

        //Act
        string token = _sut.AuthenticateAndReturnJwtToken(user.UserName, user.Password);

        //Assert
        token.Should().BeEquivalentTo("custom_token");
    }

    [Fact]
    public void AuthenticateAndReturnJwtToken_WhenUserDoesNotExist_ShouldThrowException()
    {
        //Arrange
        var user = new User()
        {
            Id = 3,
            Email = "bob@hotmail.com",
            UserName = "bob",
            Password = "123",
            CreatedAt = DateTime.Now,
            Active = true,
        };

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(MockerUsers);

        //Act
        var action = _sut.Invoking(x => _sut.AuthenticateAndReturnJwtToken(user.UserName, user.Password));

        // Assert
        action.Should()
            .Throw<AuthenticationException>()
            .WithMessage("An error occured when authenticating user bob.");
    }

    [Fact]
    public void AuthenticateAndReturnJwtToken_WhenWrongPassword_ShouldThrowException()
    {
        //Arrange
        var user = new User()
        {
            Id = 1,
            Email = "marcellus@hotmail.com",
            UserName = "marcellus",
            Password = "wrong_password",
            CreatedAt = DateTime.Now,
            Active = true,
        };

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(MockerUsers);

        //Act
        var action = _sut.Invoking(x => _sut.AuthenticateAndReturnJwtToken(user.UserName, user.Password));

        //Assert
        action.Should()
           .Throw<AuthenticationException>()
           .WithMessage("An error occured when authenticating user marcellus.");

    }
}