using MyBuyingList.Application.Common.Constants;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

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
