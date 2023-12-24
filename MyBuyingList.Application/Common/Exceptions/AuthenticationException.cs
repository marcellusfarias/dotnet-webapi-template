using MyBuyingList.Web;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class AuthenticationException : Exception, IFormattedResponseException
{
    private readonly static string _responseTitle = "An error occured when authenticating user {0}.";
    public int StatusCode => (int)HttpStatusCode.Unauthorized;
    public ErrorModel Error {  get; private set; }
    public AuthenticationException(string username, string details) : base(string.Format(_responseTitle, username))
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(string.Format(_responseTitle, username), details);
    }
}