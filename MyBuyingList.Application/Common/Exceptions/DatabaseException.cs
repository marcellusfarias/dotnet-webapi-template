namespace MyBuyingList.Application.Common.Exceptions;

//should this kind of exception be on Domain or Application layer?
public class DatabaseException : Exception, ICustomHttpException
{
    private static string defaultErrorMessage = "An database operation error occured. Message: {0}";
    private string _httpResponseMessage;
    public int HttpResponseCode => 500; //Internal Server Error
    public string HttpResponseMessage => _httpResponseMessage;
    public DatabaseException(Exception inner) : base(string.Format(defaultErrorMessage, inner.Message), inner)
    { 
        _httpResponseMessage = string.Format(defaultErrorMessage, inner.Message); 
    }
}
