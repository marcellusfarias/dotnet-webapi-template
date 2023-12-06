using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Application.Features.Users.Services;
using MyBuyingList.Infrastructure.Authentication.JwtSetup;
using MyBuyingList.Infrastructure.Authentication.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NSubstitute.ExceptionExtensions;
using MyBuyingList.Domain.Entities;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Infrastructure.Tests.UnitTests;

public class JwtProviderTests
{
    private JwtProvider _sut;
    private readonly IOptions<JwtOptions> _options;
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();

    private IFixture _fixture;

    public JwtProviderTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var options = _fixture.Create<JwtOptions>();
        _options = Options.Create(options);
        _sut = new JwtProvider(_options, _userRepositoryMock);
    }
        
    [Fact]
    public async void GenerateTokenAsync_ReturnsToken_WhenSucceded()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var returningList = _fixture.Create<List<Policy>>();

        _userRepositoryMock
            .GetUserPolicies(userId, default)
            .Returns(returningList);

        // Act
        var result = await _sut.GenerateTokenAsync(userId, default);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async void GenerateTokenAsync_ThrowsException_WhenCantConnectToDatabase()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var returningList = _fixture.Create<List<Policy>>();

        _userRepositoryMock
            .GetUserPolicies(userId, default)
            .Throws(_fixture.Create<DatabaseException>());

        //Act
        var act = async () => await _sut.GenerateTokenAsync(userId, default);

        //Assert
        await act.Should().ThrowAsync<DatabaseException>();
    }

    [Fact]
    public async void GenerateTokenAsync_ThrowsException_WhenOptionsAreEmpty()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var returningList = _fixture.Create<List<Policy>>();

        _userRepositoryMock
            .GetUserPolicies(userId, default)
            .Returns(returningList);

        var options = Substitute.For<IOptions<JwtOptions>>();
        _sut = new JwtProvider(options, _userRepositoryMock);

        //Act
        var act = async () => await _sut.GenerateTokenAsync(userId, default);

        //Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }
}
