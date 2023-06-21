using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Errors;

public class ValidationError : IApplicationError
{
    private string defaultErrorMessage = "An error occured while validating the model. Ex: {0}";
    private string _message;
    public string Message => string.Format(defaultErrorMessage, _message);
    public int ErrorCode => 400;
    public ValidationError(string message)
        => _message = message;
}
