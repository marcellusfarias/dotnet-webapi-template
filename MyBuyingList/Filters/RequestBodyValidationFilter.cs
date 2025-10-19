using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyBuyingList.Application.Common.Exceptions;
using Newtonsoft.Json;

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
            Type? modelType = context.ActionDescriptor.Parameters
                .FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body && p.ParameterType.IsClass)?.ParameterType;

            if (modelType is not null)
            {
                Type validatorType = typeof(IValidator<>).MakeGenericType(modelType);
                var validator = _serviceProvider.GetService(validatorType);

                // If there is any Validator on the DI Container, it's going to apply the validation.
                if (validator is not null)
                {
                    var httpRequestBody = context.HttpContext.Request.Body;

                    if (!httpRequestBody.CanSeek)
                    {
                        throw new Exception("Cannot seek request body.");
                    }

                    httpRequestBody.Seek(0, SeekOrigin.Begin); // Rewind the stream

                    using var reader = new StreamReader(httpRequestBody);
                    var bodyContent = await reader.ReadToEndAsync();
                    var parameterValue = JsonConvert.DeserializeObject(bodyContent, modelType);
                    var validateMethod = validatorType.GetMethod("ValidateAsync")!;

                    var validationResultTask = (Task<ValidationResult>)validateMethod.Invoke(validator, new[]
                    {
                        parameterValue,
                        CancellationToken.None
                    })!;

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
