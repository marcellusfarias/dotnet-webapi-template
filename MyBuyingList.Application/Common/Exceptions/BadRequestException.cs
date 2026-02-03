using FluentValidation.Results;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class BadRequestException : Exception, IFormattedResponseException
{
    public int StatusCode => (int)HttpStatusCode.BadRequest;

    public ErrorResponse? Error { get; }

    public BadRequestException(ValidationResult validationResult)
    {
        Error = ErrorResponse.CreateFromValidationResult(validationResult);
    }
}
