using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MyBuyingList.Infrastructure.Authentication.JwtSetup;

internal class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;
    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // TODO: if it's production environment, read from AWS Secret Key Manager.
    public void Configure(JwtOptions options)
    {
        _configuration.GetSection("JwtSettings").Bind(options);
        var jwtSecretKey = File.ReadAllText("/run/secrets/jwt_secret_key");
        if(string.IsNullOrEmpty(jwtSecretKey))
        {
            throw new ArgumentException("Please provide the JWT Secret Key");
        }
        else
        {
            options.SecretKey = jwtSecretKey;
        }
    }
}
