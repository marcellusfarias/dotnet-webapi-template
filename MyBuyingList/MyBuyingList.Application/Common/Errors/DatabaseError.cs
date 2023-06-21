using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Errors;

public class DatabaseError : IApplicationError
{
    private string defaultErrorMessage = "An error occured while operating in the database. Ex: {0}";
    private string _message;
    public string Message => string.Format(defaultErrorMessage, _message);
    public int ErrorCode => 500;
    public DatabaseError(string message)
        => _message = message;
}