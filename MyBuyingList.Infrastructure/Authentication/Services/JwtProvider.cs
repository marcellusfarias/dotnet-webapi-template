using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Features.Users;
using MyBuyingList.Infrastructure.Authentication.Constants;
using MyBuyingList.Infrastructure.Authentication.JwtSetup;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBuyingList.Infrastructure.Authentication.Services;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly JwtOptions _options;
    private readonly IUserRepository _userRepository;
    
    public JwtProvider(IOptions<JwtOptions> options, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _options = options.Value;
    }

    public async Task<(string Value, DateTimeOffset ExpiresAt)> GenerateTokenAsync(int userId, CancellationToken cancellationToken)
    {
        var permissions = await GetPermissionsAsync(userId, cancellationToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId.ToString()),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
                SecurityAlgorithms.HmacSha256Signature
            );

        permissions.ForEach(permission => claims.Add(new(CustomClaims.Permissions, permission)));

        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(_options.ExpirationTime);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            expiresAt.UtcDateTime,
            signingCredentials
            );

        string tokenValue = _tokenHandler.WriteToken(token);

        return (tokenValue, expiresAt);
    }

    private async Task<List<string>> GetPermissionsAsync(int userId, CancellationToken token)
    {
        var policies = await _userRepository.GetUserPoliciesAsync(userId, token);
        if (policies is null)
        {
            return [];
        }

        var permissions = policies
            .Select(policy => policy.Name)
            .ToList();

        return permissions;
    }

}
