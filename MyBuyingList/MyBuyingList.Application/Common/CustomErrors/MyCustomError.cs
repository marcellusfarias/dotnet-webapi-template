using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.CustomErrors;

public class MyCustomError
{
    private string _message;
    public MyCustomError(string message)
        => _message = message;

    public string Message { get { return _message; } }
}
