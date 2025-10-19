using MyBuyingList.Application.Common.Constants;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class DatabaseException : Exception, IFormattedResponseException
{
    private const string ResponseTitle = ErrorMessages.DatabaseError;
    
    public int StatusCode => (int)HttpStatusCode.InternalServerError;
    public ErrorModel Error { get; private set; }

    public DatabaseException(Exception inner) : base(string.Format(ResponseTitle), inner)
    {
        Error = ErrorModel.CreateSingleErrorDetailsModel(ResponseTitle, "Please, contact administrator");
    }
}
