using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyBuyingList.Application.Common.Interfaces;

namespace MyBuyingList.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.NameId);
            if (value is null || !int.TryParse(value, out var userId))
            {
                throw new InvalidOperationException("User ID claim is missing or invalid.");
            }

            return userId;
        }
    }
}
