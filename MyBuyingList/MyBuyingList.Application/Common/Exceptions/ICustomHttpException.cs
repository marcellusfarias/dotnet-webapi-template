using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

public interface ICustomHttpException
{
    int HttpResponseCode { get; }
    string HttpResponseMessage { get; }
}
