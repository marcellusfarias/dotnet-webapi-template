using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Interfaces.Services;
public interface ICustomAuthenticationService //TODO: think of a better name
{
    string AuthenticateAndReturnToken(string username, string password);
}
