using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Infrastructure.Auth.Constants;
using MyBuyingList.Infrastructure.Auth.JwtSetup;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBuyingList.Infrastructure.Auth.Services;

internal class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    private readonly IUserRepository _userRepository;
    public JwtProvider(IOptions<JwtOptions> options, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _options = options.Value;
    }

    public async Task<string> GenerateTokenAsync(int userId, CancellationToken cancellationToken)
    {
        var permissions = GetPermissionsAsync(userId, cancellationToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId.ToString()),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mySuperSecretKey")),
                SecurityAlgorithms.HmacSha256Signature
            );

        (await permissions).ForEach(permission => claims.Add(new(CustomClaims.Permissions, permission)));
        
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(5),
            signingCredentials
            );

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }

    private async Task<List<string>> GetPermissionsAsync(int userId, CancellationToken token)
    {
        var user = await _userRepository.GetAsync(userId, token);
        if (user == null || user.UserRoles == null)
        {
            return new List<string>();
        }

        //is this way the most performant way to do join?
        var permissions = user.UserRoles
            .SelectMany(userRole => userRole.Role.RolePolicies.Select(rolePolicy => rolePolicy.Policy.Name))
            .ToList();

        return permissions;
    }

}
