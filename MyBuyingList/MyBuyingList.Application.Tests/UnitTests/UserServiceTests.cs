using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.DTOs;
using MyBuyingList.Application.Services;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Mappings;
using MyBuyingList.Application.DTOs.UserDtos;

namespace MyBuyingList.Application.Tests.UnitTests;

public class UserServiceTests
{
    private UserService _sut;
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly IMapper _mapper;

    public UserServiceTests()
    {
        _mapper = MapperHelper.GetMapper();
        _sut = new UserService(_userRepositoryMock.Object, _mapper);
    }

    [Fact]
    public void GetAllUsers_ShouldReturnDtoList_WhenThereAreUsers()
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
        IEnumerable<GetUserDto> returnUsers = _sut.GetAllUsers();

        //Assert
        GetUserDto userDto1 = new GetUserDto() { Id = 1, UserName = "Alph", Email = "alph@hotmail.com", Active = true };
        GetUserDto userDto2 = new GetUserDto() { Id = 2, UserName = "Bob", Email = "bob@gmail.com", Active = true };
        GetUserDto userDto3 = new GetUserDto() { Id = 3, UserName = "Cindy", Email = "cindy@outlook.com", Active = false };
        IEnumerable<GetUserDto> expectedUserDtos = new List<GetUserDto>() { userDto1, userDto2, userDto3 };

        returnUsers.Should().BeEquivalentTo(expectedUserDtos);
    }

    [Fact]
    public void GetAllUsers_ShouldReturnEmptyList_WhenThereAreNoUsers()
    {
        //Arrange
        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(Enumerable.Empty<User>());

        //Act
        var users = _sut.GetAllUsers();

        //Assert
        users.Should().BeEmpty();
    }

    public static IEnumerable<object[]> ValidDtos =>
        new List<object[]>
        {
            new object[]
            {
                new User() { UserName = "Alph", Email = "alph@hotmail.com", Password = "123", Active = true },
                new CreateUserDto() { UserName = "Alph", Email = "alph@hotmail.com", Password = "123" },
            },
            new object[]
            {
                new User() { UserName = "Bob", Email = "bob@gmail.com", Password = "123", Active = false },
                new CreateUserDto() {  UserName = "Bob", Email = "bob@gmail.com", Password = "123" },
            },
            new object[]
            {
                new User() { UserName = "Cindy McAdams", Email = "cindy@gmail.com", Password = "123", Active = false },
                new CreateUserDto() { UserName = "Cindy McAdams", Email = "cindy@gmail.com", Password = "123" },
            },
        };

    //public static IEnumerable<object[]> InvalidDtos =>
    //    new List<object[]>
    //    {
    //        new object[]
    //        {
    //            new UserDto() { Id = 1, UserName = "", Email = "myemail@email.com", Active = true },
    //            "An error occured while validating the model. Exception: Validation failed: \r\n -- UserName: Please specify a name. Severity: Error"
    //        },
    //        new object[]
    //        {
    //            new UserDto() { Id = 1, UserName = "John", Email = string.Empty, Active = true },
    //            "An error occured while validating the model. Exception: Validation failed: \r\n -- Email: Please specify an email address. Severity: Error"
    //        },
    //        new object[]
    //        {
    //            new UserDto() { Id = 1, UserName = "John", Email = "myemail@email.com", Active = true },
    //            "An error occured while validating the model. Exception: Validation failed: \r\n -- Password: Please specify a password. Severity: Error"
    //        },
    //        new object[]
    //        {
    //            new UserDto() { Id = -1, UserName = "John", Email = "myemail@email.com", Active = false },
    //            "An error occured while validating the model. Exception: Validation failed: \r\n -- Id: Id should have a positive value. Severity: Error"
    //        }
    //};

    ////TODO: try to use this: 
    //// https://stackoverflow.com/questions/54219742/mocking-ef-core-dbcontext-and-dbset
    //[Theory]
    //[MemberData(nameof(ValidDtos))]
    //public void Create_ShouldHaveNoErrors_WhenDtoIsValid(User user, CreateUserDto userDto)
    //{
    //    //Arrange
    //    _userRepositoryMock
    //        .Setup(x => x.Add(It.IsAny<User>()))
    //        .Returns(() => 1);

    //    //Act
    //    var createdId = _sut.Create(userDto);

    //    //Assert
    //    _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    //    createdId.Should().Be(1);
    //}

    ////TODO: pass as parameter many DTO with different errors.
    //[Theory]
    //[MemberData(nameof(InvalidDtos))]
    //public void Create_ShouldThrowException_WhenDtoIsInvalid(UserDto newUser, string errorMessage)
    //{
    //    //Arrange

    //    //Act
    //    //var action = _sut.Invoking(x => _sut.Create(newUser));

    //    //// Assert
    //    //action.Should()
    //    //    .Throw<CustomValidationException>()
    //    //    .WithMessage(errorMessage);
    //}

    //[Theory]
    //[MemberData(nameof(ValidDtos))]
    //public void Update_ShouldHaveNoErrors_WhenDtoIsValid(User user, UserDto userDto)
    //{
    //    //Arrange
    //    _userRepositoryMock
    //        .Setup(x => x.Edit(user));

    //    //Act
    //    //_sut.Update(userDto);

    //    //// Assert
    //    //_userRepositoryMock.Verify(r => r.Edit(It.IsAny<User>()), Times.Once);
    //}

    //[Theory]
    //[MemberData(nameof(InvalidDtos))]
    //public void Update_ShouldThrowException_WhenDtoIsInvalid(UserDto newUser, string errorMessage)
    //{
    //    //Arrange

    //    //Act
    //    //var action = _sut.Invoking(x => _sut.Update(newUser));

    //    //// Assert
    //    //action.Should()
    //    //    .Throw<CustomValidationException>()
    //    //    .WithMessage(errorMessage);
    //}

    //[Theory]
    //[MemberData(nameof(ValidDtos))]
    //public void Delete_ShouldHaveNoErrors_WhenDtoIsValid(User user, UserDto userDto)
    //{
    //    //Arrange
    //    _userRepositoryMock
    //        .Setup(x => x.Delete(user));

    //    ////Act
    //    //_sut.Delete(userDto);

    //    //// Assert
    //    //_userRepositoryMock.Verify(r => r.Delete(It.IsAny<User>()), Times.Once);
    //}

    //[Theory]
    //[MemberData(nameof(InvalidDtos))]
    //public void Delete_ShouldThrowException_WhenDtoIsInvalid(UserDto newUser, string errorMessage)
    //{
    //    //Arrange

    //    //Act
    //    //var action = _sut.Invoking(x => _sut.Delete(newUser));

    //    //// Assert
    //    //action.Should()
    //    //    .Throw<CustomValidationException>()
    //    //    .WithMessage(errorMessage);
    //}
}