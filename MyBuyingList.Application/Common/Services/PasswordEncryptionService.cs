using System.Security.Cryptography;
using System.Text;
using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace MyBuyingList.Application.Common.Services;

// TODO: no reason for this to be a service, should be a static helper class for crypto/hashing functions
public class PasswordEncryptionService : IPasswordEncryptionService
{
    private const int WorkingFactor = 12;

    public string HashPassword(string password)
    {
        try
        {
            return BC.HashPassword(password, WorkingFactor);
        }
        catch (Exception ex)
        {
            throw new InternalServerErrorException(ex, "Failure while creating password. Please, contact administrator.");
        }
        
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BC.Verify(password, hashedPassword);
        }
        catch (Exception ex)
        {
            throw new InternalServerErrorException(ex, "Failure while validating passwords. Please, contact administrator.");
        }
    }

    public string ComputeRefreshTokenHash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLower();
    }
}
