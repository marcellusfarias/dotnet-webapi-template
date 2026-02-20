namespace MyBuyingList.Infrastructure.Authentication.JwtSetup;

/// <summary>
/// Admin user configuration options.
/// </summary>
public class AdminSettings
{
    public const string SectionName = "AdminSettings";

    /// <summary>Plaintext password used to seed/update the admin user.</summary>
    public required string Password { get; init; }

    /// <summary>Email address used to seed/update the admin user.</summary>
    public required string Email { get; init; }
}
