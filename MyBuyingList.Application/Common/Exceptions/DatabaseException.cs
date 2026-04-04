using MyBuyingList.Application.Common.Constants;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

// This exception exists so the ExceptionMiddleware doesn't expose internal implementation details on the response
// We should catch other expected exceptions and handle them properly, and use this only for unexpected database errors
public class DatabaseException : Exception, IFormattedResponseException
{
    private const string ResponseTitle = ErrorMessages.DatabaseError;
    
    public int StatusCode => (int)HttpStatusCode.InternalServerError;
    public ErrorResponse Error { get; }

    public DatabaseException(Exception inner) : base(ResponseTitle, inner)
    {
        Error = ErrorResponse.CreateSingleErrorDetail(ResponseTitle, "Please, contact administrator");
    }
}
