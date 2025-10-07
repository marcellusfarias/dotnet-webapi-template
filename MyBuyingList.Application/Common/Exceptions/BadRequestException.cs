using FluentValidation.Results;
using System.Net;

namespace MyBuyingList.Application.Common.Exceptions;

public class BadRequestException : Exception, IFormattedResponseException
{
    public int StatusCode => (int)HttpStatusCode.BadRequest;

    public ErrorModel? Error { get; private set; }

    public BadRequestException(ValidationResult validationResult) : base() 
    {
        Error = ErrorModel.CreateFromValidationResult(validationResult);
    }
}
