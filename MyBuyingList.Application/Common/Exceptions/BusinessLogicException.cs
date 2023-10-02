using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Application.Common.Exceptions;

// generic one. Think about this in the future.
public  class BusinessLogicException : Exception, ICustomHttpException
{
    private static string defaultErrorMessage = "An error occured while processing the request. Message: {0}";
    private string _httpResponseMessage;
    public int HttpResponseCode => 422; //UnprocessableEntity
    public string HttpResponseMessage => _httpResponseMessage;
    public BusinessLogicException(string message) : base(string.Format(defaultErrorMessage, message)) { _httpResponseMessage = string.Format(defaultErrorMessage, message); }
}