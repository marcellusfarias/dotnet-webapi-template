using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

public class ResourceNotFoundException : Exception
{
    private static string defaultErrorMessage = "Resource not found.";
    public ResourceNotFoundException() : base(defaultErrorMessage) { }
    public ResourceNotFoundException(Exception inner) : base(string.Format(defaultErrorMessage), inner) { }
}
