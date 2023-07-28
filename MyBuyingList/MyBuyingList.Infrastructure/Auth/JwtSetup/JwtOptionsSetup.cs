using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MyBuyingList.Infrastructure.Auth.JwtSetup;

internal class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;
    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection("JwtSettings").Bind(options);
    }
}
