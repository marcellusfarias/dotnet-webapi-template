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

namespace MyBuyingList.Application.Tests.UnitTests;

public class UserServiceTests
{
    private UserService _sut;
    private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
    private readonly IMapper _mapper;
    private readonly IValidator<UserDto> _validatorMock = new UserValidator(); //should not mock: https://docs.fluentvalidation.net/en/latest/testing.html#

    public UserServiceTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DefaultProfile());
        });

        _mapper = mappingConfig.CreateMapper();

        _sut = new UserService(_userRepositoryMock.Object, _mapper, _validatorMock);

        // Other way if not using Profile
        // https://stackoverflow.com/questions/36074324/how-to-mock-an-automapper-imapper-object-in-web-api-tests-with-structuremap-depe
        //_mapper.Setup(x => x.Map<IEnumerable<UserDto>>(It.IsAny<IEnumerable<User>>()))
        //    .Returns((IEnumerable<User> sources) =>
        //    {
        //        // abstract mapping function code here, return instance of DestinationClass
        //        var returnValue = new List<UserDto>();

        //        foreach (var source in sources)
        //        {
        //            returnValue.Add(new UserDto()
        //            {
        //                Id = source.Id,
        //                UserName = source.UserName,
        //                Password = source.Password,
        //                Email = source.Email,
        //                Active = source.Active
        //            });
        //        }

        //        return returnValue;
        //    });
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

    public static IEnumerable<object[]> InvalidDtos =>
        new List<object[]>
        {
            new object[] 
            { 
                new UserDto() { Id = 1, UserName = "", Password = "123", Email = "myemail@email.com", Active = true }, 
                "An error occured while validating the model. Exception: Validation failed: \r\n -- UserName: Please specify a name. Severity: Error" 
            },
            new object[]
            {
                new UserDto() { Id = 1, UserName = "John", Password = "123", Email = string.Empty, Active = true },
                "An error occured while validating the model. Exception: Validation failed: \r\n -- Email: Please specify an email address. Severity: Error"
            }
        };

    //TODO: pass as parameter many DTO with different errors.
    [Theory]
    [MemberData(nameof(InvalidDtos))]
    public void Create_ShouldThrowException_WhenDtoIsInvalid(UserDto newUser, string errorMessage)
    {
        //Arrange

        //Act
        var action = _sut.Invoking(x => _sut.Create(newUser));

        // Assert
        action.Should()
            .Throw<CustomValidationException>()
            .WithMessage(errorMessage);
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

    [Theory]
    [MemberData(nameof(InvalidDtos))]
    public void Update_ShouldThrowException_WhenDtoIsInvalid(UserDto newUser, string errorMessage)
    {
        //Arrange
       
        //Act
        var action = _sut.Invoking(x => _sut.Update(newUser));

        // Assert
        action.Should()
            .Throw<CustomValidationException>()
            .WithMessage(errorMessage);
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

    [Theory]
    [MemberData(nameof(InvalidDtos))]
    public void Delete_ShouldThrowException_WhenDtoIsInvalid(UserDto newUser, string errorMessage)
    {
        //Arrange
        
        //Act
        var action = _sut.Invoking(x => _sut.Delete(newUser));

        // Assert
        action.Should()
            .Throw<CustomValidationException>()
            .WithMessage(errorMessage);
    }
}