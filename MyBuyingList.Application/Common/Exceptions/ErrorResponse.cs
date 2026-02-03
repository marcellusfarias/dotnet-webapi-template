using FluentValidation.Results;

namespace MyBuyingList.Application.Common.Exceptions;

public class ErrorResponse
{
    public required List<ErrorDetail> Errors { get; init; }

    public static ErrorResponse CreateSingleErrorDetail(string title, string detail)
    {
        var item = new ErrorDetail
        {
            Title = title,
            Detail = detail,
        };

        return new ErrorResponse
        {
            Errors = [item]
        };
    }

    public static ErrorResponse CreateFromValidationResult(ValidationResult validationResult)
    {
        var errorDetails = new List<ErrorDetail>();

        validationResult
           .Errors
           .ForEach(x =>
               errorDetails.Add(
                   new ErrorDetail()
                   {
                       Title = $"Error validating property '{x.PropertyName}'.",
                       Detail = x.ErrorMessage
                   }));

        return new ErrorResponse { Errors = errorDetails };
    }
}

public class ErrorDetail
{
    public required string Title { get; set; }

    public required string Detail { get; set; }
}