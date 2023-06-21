using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Errors;

//tested the Result nomad, not following with it
public interface IApplicationError
{
    string Message { get; }
    int ErrorCode { get; }
}