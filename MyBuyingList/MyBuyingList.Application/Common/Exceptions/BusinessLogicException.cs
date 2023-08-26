using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

// generic one. Think about this in the future.
public  class BusinessLogicException : Exception
{
    private static string defaultErrorMessage = "An error occured while validating the model. Message: {0}";
    public BusinessLogicException() : base(defaultErrorMessage) { }
    public BusinessLogicException(string message) : base(string.Format(defaultErrorMessage, message)) { }
    public BusinessLogicException(Exception inner) : base(string.Format(defaultErrorMessage, inner.Message), inner) { }
}