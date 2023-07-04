using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Application.Services;
using MyBuyingList.Domain.Entities;
using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyBuyingList.Application.Tests.UnitTests;

public class UserServiceTests
{
    private UserService _sut;
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
    private readonly IValidator<UserDto> _validatorMock = new UserValidator(); //should not mock: https://docs.fluentvalidation.net/en/latest/testing.html#


    public UserServiceTests()
    {
        _sut = new UserService(_userRepositoryMock.Object, _mapperMock.Object, _validatorMock);

        // TODO: Try to use an AutoMapperProfile according to the second answer here:
        // https://stackoverflow.com/questions/36074324/how-to-mock-an-automapper-imapper-object-in-web-api-tests-with-structuremap-depe
        _mapperMock.Setup(x => x.Map<IEnumerable<UserDto>>(It.IsAny<IEnumerable<User>>()))
            .Returns((IEnumerable<User> sources) =>
            {
                // abstract mapping function code here, return instance of DestinationClass
                var returnValue = new List<UserDto>();

                foreach (var source in sources)
                {
                    returnValue.Add(new UserDto()
                    {
                        Id = source.Id,
                        UserName = source.UserName,
                        Password = source.Password,
                        Email = source.Email,
                        Active = source.Active
                    });
                }

                return returnValue;
            });
    }

    [Fact]
    public void List_ShouldReturnDtoList_WhenThereAreUsers()
    {
        //Arrange
        User user1 = new User() { Id = 1, UserName = "Alph", Email = "alph@hotmail.com", Password = "123", Active = true };
        User user2 = new User() { Id = 2, UserName = "Bob", Email = "bob@gmail.com", Password = "321", Active = true };
        User user3 = new User() { Id = 3, UserName = "Cindy", Email = "cindy@outlook.com", Password = "cindy123", Active = false };

        IEnumerable<User> mockedUsers = new List<User>() { user1, user2, user3 };

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(mockedUsers);

        //Act
        IEnumerable<UserDto> returnUsers = _sut.List();

        //Assert
        UserDto userDto1 = new UserDto() { Id = 1, UserName = "Alph", Password = "123", Email = "alph@hotmail.com", Active = true };
        UserDto userDto2 = new UserDto() { Id = 2, UserName = "Bob", Password = "321", Email = "bob@gmail.com", Active = true };
        UserDto userDto3 = new UserDto() { Id = 3, UserName = "Cindy", Password = "cindy123", Email = "cindy@outlook.com", Active = false };
        IEnumerable<UserDto> expectedUserDtos = new List<UserDto>() { userDto1, userDto2, userDto3 };

        returnUsers.Should().BeEquivalentTo(expectedUserDtos);
    }

    [Fact]
    public void List_ShouldReturnEmptyList_WhenThereAreNoUsers()
    {
        //Arrange
        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(Enumerable.Empty<User>());

        //Act
        var users = _sut.List();

        //Assert
        users.Should().BeEmpty();
    }

    //TODO: try to use this: 
    // https://stackoverflow.com/questions/54219742/mocking-ef-core-dbcontext-and-dbset
    [Fact]
    public void Create_ShouldHaveNoErrors_WhenDtoIsValid()
    {
        //Arrange
        User user1 = new User() { Id = 1, UserName = "Alph", Email = "alph@hotmail.com", Password = "123", Active = true };
        UserDto userDto1 = new UserDto() { Id = 1, UserName = "Alph", Password = "123", Email = "alph@hotmail.com", Active = true };

        _userRepositoryMock
            .Setup(x => x.Add(user1));

        //Act
        _sut.Create(userDto1);

        //Assert
        _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    //TODO: pass as parameter many DTO with different errors.
    [Fact]
    public void Create_ShouldThrowException_WhenDtoIsInvalid()
    {
        //Arrange
        UserDto userDto1 = new UserDto() { Id = 1, UserName = "", Password = "123", Email = "myemail@email.com", Active = true };

        //Act
        var action = _sut.Invoking(x => _sut.Create(userDto1));

        // Assert
        action.Should()
            .Throw<CustomValidationException>()
            .WithMessage("An error occured while validating the model. Exception: Validation failed: \r\n -- UserName: Please specify a name. Severity: Error");
    }

    [Fact]
    public void Update_ShouldHaveNoErrors_WhenDtoIsValid()
    {
        //Arrange
        User user1 = new User() { Id = 1, UserName = "Alph", Email = "alph@hotmail.com", Password = "123", Active = true };
        UserDto userDto1 = new UserDto() { Id = 1, UserName = "Alph", Password = "123", Email = "alph@hotmail.com", Active = true };
        _userRepositoryMock
            .Setup(x => x.Edit(user1));

        //Act
        _sut.Update(userDto1);

        // Assert
        _userRepositoryMock.Verify(r => r.Edit(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenDtoIsInvalid()
    {
        //Arrange
        UserDto userDto1 = new UserDto() { Id = 1, UserName = string.Empty, Password = "123", Email = "myemail@email.com", Active = true };

        //Act
        var action = _sut.Invoking(x => _sut.Update(userDto1));

        // Assert
        action.Should()
            .Throw<CustomValidationException>()
            .WithMessage("An error occured while validating the model. Exception: Validation failed: \r\n -- UserName: Please specify a name. Severity: Error");
    }

    [Fact]
    public void Delete_ShouldHaveNoErrors_WhenDtoIsValid()
    {
        //Arrange
        User user1 = new User() { Id = 1, UserName = "Alph", Email = "alph@hotmail.com", Password = "123", Active = true };
        UserDto userDto1 = new UserDto() { Id = 1, UserName = "Alph", Password = "123", Email = "alph@hotmail.com", Active = true };
        _userRepositoryMock
            .Setup(x => x.Delete(user1));

        //Act
        _sut.Delete(userDto1);

        // Assert
        _userRepositoryMock.Verify(r => r.Delete(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void Delete_ShouldThrowException_WhenDtoIsInvalid()
    {
        //Arrange
        UserDto userDto1 = new UserDto() { Id = 1, UserName = string.Empty, Email = "myemail@hotmail.com", Password = "123", Active = true };

        //Act
        var action = _sut.Invoking(x => _sut.Delete(userDto1));

        // Assert
        action.Should()
            .Throw<CustomValidationException>()
            .WithMessage("An error occured while validating the model. Exception: Validation failed: \r\n -- UserName: Please specify a name. Severity: Error");
    }
}