using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

//should this kind of exception be on Domain or Application layer?
public class ValidationException : Exception
{
    private static string defaultErrorMessage = "An error occured while validating the model. Ex: {0}";
    public ValidationException(string message) : base(string.Format(defaultErrorMessage, message)) { }
}
