using FluentValidation;
using MyBuyingList.Application.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace MyBuyingList.Application.Common.Extensions;

// not problem with static method since we should not mock FluentValidation: https://docs.fluentvalidation.net/en/latest/testing.html#
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