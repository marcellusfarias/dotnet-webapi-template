using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface ILoginService
{
    Task<string> AuthenticateAndReturnJwtTokenAsync(string username, string password, CancellationToken cancellationToken);
}
