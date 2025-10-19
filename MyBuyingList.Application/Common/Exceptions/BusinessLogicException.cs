using MyBuyingList.Application.Common.Constants;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public  class BusinessLogicException : Exception, IFormattedResponseException
{
    private const string ResponseTitle = ErrorMessages.BusinessLogicError;
    public int StatusCode => (int)HttpStatusCode.UnprocessableEntity;
    public ErrorModel Error { get; private set; }
    public BusinessLogicException(string details) : base(ResponseTitle) 
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(ResponseTitle, details);
    }
}