﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface ILoginService
{
    string AuthenticateAndReturnJwtToken(string username, string password);
}