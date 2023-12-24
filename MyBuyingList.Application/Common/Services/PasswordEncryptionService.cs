using MyBuyingList.Application.Common.Exceptions;
using MyBuyingList.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace MyBuyingList.Application.Common.Services;

public class PasswordEncryptionService : IPasswordEncryptionService
{
    private const int _workingFactor = 12;

    public PasswordEncryptionService() { }

    public string HashPassword(string password)
    {
        try
        {
            return BC.HashPassword(password, _workingFactor);
        }
        catch (Exception ex)
        {
            throw new InternalServerErrorException(ex, "Failure while creating password. Please, contact administrator.");
        }
        
    }

    public bool VerifyPasswordsAreEqual(string password, string hashedPassword)
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
}
