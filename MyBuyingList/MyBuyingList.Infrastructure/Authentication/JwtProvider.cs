using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using MyBuyingList.Application.Common.Interfaces.Repositories;
using MyBuyingList.Application.Common.Interfaces.Services;
using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    private readonly IUserRepository _userRepository;

    public JwtProvider(IOptions<JwtOptions> options, IUserRepository userRepository)
    {
        _options = options.Value;
        _userRepository = userRepository;
    }

    public string AuthenticateAndReturnToken(string email, string password)
    {
        User? user = _userRepository.GetAuthenticationDataByEmail(email);
        if (user == null)
            throw new AuthenticationException(new Exception(), email);

        //TODO: unhash password

        if (password == user.Password)
            return Generate(user.Id);
        else
            throw new AuthenticationException(new Exception(), email);
    }

    private List<string> GetPermissions(int userId)
    {
        var user = _userRepository.Get(userId);
        if (user == null)
            return new List<string>();

        var groups = user.GroupsCreatedBy;
        var userRoles = user.UserRoles;
        if (userRoles == null)
            return new List<string>();

        List<Role> roles = new List<Role>();
        userRoles.ToList().ForEach(x => roles.Add(x.Role));

        List<string> permissions = new List<string>();
        roles.ToList().ForEach(x => x.RolePolicies.ToList().ForEach(y => permissions.Add(y.Policy.Name)));

        return permissions;
    }

    private string Generate(int userId)
    {
        List<string> permissions = GetPermissions(userId); 

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId.ToString()),
        };

        permissions.ForEach(permission => claims.Add(new("permissions", permission)));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mySuperSecretKey")),
                SecurityAlgorithms.HmacSha256Signature
            );

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
}
