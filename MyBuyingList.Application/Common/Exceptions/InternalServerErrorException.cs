using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class InternalServerErrorException : Exception, IFormattedResponseException
{
    private const string ResponseTitle = "An unexpected error occured.";
    
    public int StatusCode => (int)HttpStatusCode.InternalServerError;
    public ErrorModel Error { get; private set; }
    
    public InternalServerErrorException(Exception inner, string details) : base(ResponseTitle, inner)
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(ResponseTitle, details);
    }
}
