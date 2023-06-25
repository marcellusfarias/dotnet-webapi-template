namespace MyBuyingList.Application.Common.Exceptions;

//should this kind of exception be on Domain or Application layer?
public class DatabaseException : Exception
{
    private static string defaultErrorMessage = "An error occured while operating in the database. Ex: {0}";
    public DatabaseException(Exception inner) : base(string.Format(defaultErrorMessage, inner.Message), inner) { }
}
