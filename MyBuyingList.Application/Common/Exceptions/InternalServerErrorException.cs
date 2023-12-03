namespace MyBuyingList.Application.Common.Exceptions;

// Use this exception when an unexpected error happened.
public class InternalServerErrorException : Exception, ICustomHttpException
{
    private static string defaultErrorMessage = "{0}";
    private string _httpResponseMessage;
    public int HttpResponseCode => 500; //Internal Server Error
    public string HttpResponseMessage => _httpResponseMessage;
    public InternalServerErrorException(Exception inner, string message) : base(string.Format(defaultErrorMessage, inner.Message), inner)
    {
        _httpResponseMessage = string.Format(defaultErrorMessage, message);
    }
}
