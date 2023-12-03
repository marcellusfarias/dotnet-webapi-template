using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Interfaces;

public interface IPasswordEncryptionService
{
    string HashPassword(string password);
    bool VerifyPasswordsAreEqual(string password, string hashedPassword);
}
