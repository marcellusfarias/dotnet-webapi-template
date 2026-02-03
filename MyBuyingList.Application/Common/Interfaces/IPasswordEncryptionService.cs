namespace MyBuyingList.Application.Common.Interfaces;

public interface IPasswordEncryptionService
{
    string HashPassword(string password);
    bool VerifyPasswordsAreEqual(string password, string hashedPassword);
}
