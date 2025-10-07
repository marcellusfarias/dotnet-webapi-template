using MyBuyingList.Application.Common.Constants;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public  class BusinessLogicException : Exception, IFormattedResponseException
{
    private readonly static string _responseTitle = ErrorMessages.BusinessLogicError;
    public int StatusCode => (int)HttpStatusCode.UnprocessableEntity;
    public ErrorModel Error { get; private set; }
    public BusinessLogicException(string details) : base(_responseTitle) 
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(_responseTitle, details);
    }
}