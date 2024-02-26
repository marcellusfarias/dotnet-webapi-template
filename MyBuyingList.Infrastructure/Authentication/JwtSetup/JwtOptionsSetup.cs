using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MyBuyingList.Infrastructure.Authentication.JwtSetup;

internal class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecretFilePath;

    // TODO: this should support LOCAL (reading the secrets from the local\secrets folder
    // DEVELOPMENT (i.e. running on docker and reading from secrets
    // and PRODUCTION (reading from Aws Secret Manager.
    public JwtOptionsSetup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        _configuration = configuration;

        if (hostEnvironment.IsDevelopment())
        {
            var basePath = System.IO.Path.GetFullPath(@"..\..\..\..\")!;
            _jwtSecretFilePath = Path.Combine(basePath, @"local\secrets\jwt_secret_key.txt");
        }
        else
        {
            _jwtSecretFilePath = "/run/secrets/jwt_secret_key";
        }
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection("JwtSettings").Bind(options);
        var jwtSecretKey = File.ReadAllText(_jwtSecretFilePath);
        if (string.IsNullOrEmpty(jwtSecretKey))
        {
            throw new ArgumentException("Please provide the JWT Secret Key");
        }
        else
        {
            options.SecretKey = jwtSecretKey;
        }
    }
}
