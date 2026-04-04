using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBuyingList.Application.Common.Exceptions;

namespace MyBuyingList.Web.Filters;

public class RequestBodyValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RequestBodyValidationFilter> _logger;

    public RequestBodyValidationFilter(IServiceProvider serviceProvider, ILogger<RequestBodyValidationFilter> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// This is a filter intended to substitute FluentValidation.AspNetCore automatic validation and add support to async methods.
    /// It validates automatically only body parameters (not query string nor route), but it may be extended in the future as needed.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var bodyParameter = context.ActionDescriptor.Parameters
                .FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body && p.ParameterType.IsClass);

            if (bodyParameter is not null)
            {
                Type validatorType = typeof(IValidator<>).MakeGenericType(bodyParameter.ParameterType);
                var validator = _serviceProvider.GetService(validatorType);

                // If there is any Validator on the DI Container, it's going to apply the validation.
                if (validator is not null
                    && context.ActionArguments.TryGetValue(bodyParameter.Name, out var parameterValue)
                    && parameterValue is not null)
                {
                    var validateMethod = validatorType.GetMethod("ValidateAsync")!;

                    var validationResultTask = (Task<ValidationResult>)validateMethod.Invoke(validator,
                    [
                        parameterValue,
                        CancellationToken.None
                    ])!;

                    var validationResult = await validationResultTask;
                    if (!validationResult.IsValid)
                    {
                        throw new BadRequestException(validationResult);
                    }
                }
            }
        }
        catch(BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected error while validating user input. Ex: {ExMessage}", ex.Message);
            throw new InternalServerErrorException(ex, "Error while validating user input.");
        }

        await next();
    }
}
