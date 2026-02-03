using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class AuthenticationException : Exception, IFormattedResponseException
{
    private const string ResponseTitle = "An error occurred when authenticating user {0}.";
    public int StatusCode => (int)HttpStatusCode.Unauthorized;
    public ErrorResponse Error { get; }
    public AuthenticationException(string username, string details) : base(string.Format(ResponseTitle, username))
    {
        Error = ErrorResponse.CreateSingleErrorDetail(string.Format(ResponseTitle, username), details);
    }
}