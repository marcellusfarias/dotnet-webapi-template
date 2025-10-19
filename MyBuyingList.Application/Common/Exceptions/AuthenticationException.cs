using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class AuthenticationException : Exception, IFormattedResponseException
{
    private const string ResponseTitle = "An error occured when authenticating user {0}.";
    public int StatusCode => (int)HttpStatusCode.Unauthorized;
    public ErrorModel Error {  get; private set; }
    public AuthenticationException(string username, string details) : base(string.Format(ResponseTitle, username))
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(string.Format(ResponseTitle, username), details);
    }
}