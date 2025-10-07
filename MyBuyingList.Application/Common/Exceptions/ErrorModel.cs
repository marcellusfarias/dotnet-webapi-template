﻿using FluentValidation.Results;

namespace MyBuyingList.Application;

public class ErrorModel
{
    public required List<ErrorDetails> Errors { get; set; }

    public static ErrorModel CreateSingleErrorDetailsModel(string title, string detail)
    {
        var item = new ErrorDetails
        {
            Title = title,
            Detail = detail,
        };

        return new ErrorModel
        {
            Errors = new List<ErrorDetails> { item }
        };
    }

    public static ErrorModel CreateFromValidationResult(ValidationResult validationResult)
    {
        var errorDetails = new List<ErrorDetails>();

        validationResult
           .Errors
           .ForEach(x =>
               errorDetails.Add(
                   new ErrorDetails()
                   {
                       Title = $"Error validating property '{x.PropertyName}'.",
                       Detail = x.ErrorMessage
                   }));

        return new ErrorModel { Errors = errorDetails };
    }
}

public class ErrorDetails
{
    public required string Title { get; set; }

    public required string Detail { get; set; }
}