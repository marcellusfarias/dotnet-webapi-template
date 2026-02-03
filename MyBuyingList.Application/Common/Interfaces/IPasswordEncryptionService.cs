namespace MyBuyingList.Application.Common.Interfaces;

/// <summary>
/// Provides password hashing and verification functionality.
/// </summary>
public interface IPasswordEncryptionService
{
    /// <summary>
    /// Hashes a plain text password.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies that a plain text password matches a hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if passwords match; otherwise, false.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
