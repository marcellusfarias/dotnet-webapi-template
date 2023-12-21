using MyBuyingList.Application.Features.Users.DTOs;
using MyBuyingList.Application.Features.Users.Mappers;
using MyBuyingList.Domain.Entities;
using System.Net.Mail;

namespace MyBuyingList.Application.Tests.Features.Users;

public class UserMapperTests
{
    private IFixture _fixture;

    public UserMapperTests()
    {
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
    public void UserMapperDomainToDto_ReturnsGetUserDto_WhenDomainIsValid()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var expectedDto = new GetUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Active = user.Active,
            UserName = user.UserName,
        };
        // Act
        var dto = user.ToGetUserDto();

        // Assert
        dto.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public void UserMapperDtoToDomain_ReturnsUser_WhenDtoIsValid()
    {
        // Arrange
        var dto = _fixture.Create<CreateUserDto>();
        var expectedUser = new User
        {
            Id = 0,
            Email = dto.Email,
            Active = true,
            UserName = dto.UserName,
            Password = dto.Password,
        };
        // Act
        var user = dto.ToUser(true);

        // Assert
        user.Should().BeEquivalentTo(expectedUser);
    }
}
