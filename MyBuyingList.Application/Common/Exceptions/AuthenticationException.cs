namespace MyBuyingList.Application.Common.Exceptions;

public class AuthenticationException : Exception, ICustomHttpException
{
    private static string defaultErrorMessage = "An error occured when authenticating user {0}. {1}";
    private string _httpResponseMessage;
    public int HttpResponseCode => 401; //Unauthorized
    public string HttpResponseMessage => _httpResponseMessage;
    public AuthenticationException(string username, string message) : base(string.Format(defaultErrorMessage, username, message))
    {
        _httpResponseMessage = string.Format(defaultErrorMessage, username, message);
    }    
}