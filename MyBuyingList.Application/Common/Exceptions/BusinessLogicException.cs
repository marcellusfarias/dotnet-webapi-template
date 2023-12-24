using MyBuyingList.Web;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public  class BusinessLogicException : Exception, IFormattedResponseException
{
    private readonly static string _responseTitle = "An error occured while processing the request.";
    public int StatusCode => (int)HttpStatusCode.UnprocessableEntity;
    public ErrorModel Error { get; private set; }
    public BusinessLogicException(string details) : base(_responseTitle) 
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(_responseTitle, details);
    }
}