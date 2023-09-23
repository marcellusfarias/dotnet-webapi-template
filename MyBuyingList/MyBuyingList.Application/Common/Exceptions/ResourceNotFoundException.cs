using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

public class ResourceNotFoundException : Exception, ICustomHttpException
{
    private static string defaultErrorMessage = "Resource not found.";
    public int HttpResponseCode => 404; // Not found
    public string HttpResponseMessage => defaultErrorMessage;
    public ResourceNotFoundException() : base(defaultErrorMessage) { }    
}
