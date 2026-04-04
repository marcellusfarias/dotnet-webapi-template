using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyBuyingList.Web.Services;

namespace MyBuyingList.Web.Tests.UnitTests.Services;

public class CurrentUserServiceTests
{
    private static IHttpContextAccessor BuildAccessor(string? nameIdValue)
    {
        var claims = nameIdValue is null
            ? []
            : new[] { new Claim(JwtRegisteredClaimNames.NameId, nameIdValue) };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        return accessor;
    }

    [Fact]
    public void UserId_WithValidClaim_ReturnsUserId()
    {
        var accessor = BuildAccessor("42");
        var sut = new CurrentUserService(accessor);

        sut.UserId.Should().Be(42);
    }

    [Fact]
    public void UserId_WithMissingClaim_ThrowsInvalidOperationException()
    {
        var accessor = BuildAccessor(null);
        var sut = new CurrentUserService(accessor);

        var act = () => sut.UserId;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UserId_WithNonNumericClaim_ThrowsInvalidOperationException()
    {
        var accessor = BuildAccessor("not-a-number");
        var sut = new CurrentUserService(accessor);

        var act = () => sut.UserId;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UserId_WithNullHttpContext_ThrowsInvalidOperationException()
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns((HttpContext?)null);
        var sut = new CurrentUserService(accessor);

        var act = () => sut.UserId;

        act.Should().Throw<InvalidOperationException>();
    }
}
