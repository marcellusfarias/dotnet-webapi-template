using FluentValidation;
using MyBuyingList.Application.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace MyBuyingList.Application.Common.Extensions;

public static class ValidationExtensions
{
    public static void ValidateAndThrowCustomException<T>(this IValidator<T> validator, T instance)
    {
        var res = validator.Validate(instance);

        if (!res.IsValid)
        {
            var ex = new ValidationException(res.Errors);
            throw new CustomValidationException(ex);
        }
    }
}