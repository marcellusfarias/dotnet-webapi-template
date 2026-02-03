using FluentValidation.Results;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class BadRequestException : Exception, IFormattedResponseException
{
    public int StatusCode => (int)HttpStatusCode.BadRequest;

    public ErrorModel? Error { get; }

    public BadRequestException(ValidationResult validationResult)
    {
        Error = ErrorModel.CreateFromValidationResult(validationResult);
    }
}
