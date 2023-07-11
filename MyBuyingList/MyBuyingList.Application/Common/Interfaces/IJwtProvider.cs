using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Interfaces;

public interface IJwtProvider
{
    string Generate(string username);
}
