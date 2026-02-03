namespace MyBuyingList.Infrastructure.Authentication.JwtSetup;

/// <summary>
/// JWT authentication configuration options.
/// </summary>
public class JwtOptions
{
    /// <summary>Token issuer (iss claim).</summary>
    public required string Issuer { get; set; }

    /// <summary>Token audience (aud claim).</summary>
    public required string Audience { get; set; }

    /// <summary>Token expiration time in seconds.</summary>
    public required int ExpirationTime { get; set; }

    /// <summary>Secret key used to sign tokens (minimum 256 bits).</summary>
    public required string SigningKey { get; set; }
}
