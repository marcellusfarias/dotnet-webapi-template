using MyBuyingList.Web;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class InternalServerErrorException : Exception, IFormattedResponseException
{
    private readonly static string _responseTitle = "An unexpected error occured.";
    
    public int StatusCode => (int)HttpStatusCode.InternalServerError;
    public ErrorModel Error { get; private set; }
    
    public InternalServerErrorException(Exception inner, string details) : base(_responseTitle, inner)
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(_responseTitle, details);
    }
}
