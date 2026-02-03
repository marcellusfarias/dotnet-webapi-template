using MyBuyingList.Application;
using MyBuyingList.Application.Common.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace MyBuyingList.Web.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        if (exception is OperationCanceledException)
        {
            logger.LogInformation("Aborting request because it was cancelled");
            context.Abort();
            return Task.CompletedTask;
        }

        int code = (int)HttpStatusCode.InternalServerError;
        ErrorResponse? errorModel = null;

        if (exception is IFormattedResponseException formattedException)
        {
            code = formattedException.StatusCode;
            errorModel = formattedException.Error;
        }

        logger.LogError("StatusCode: {Code}", code);
        logger.LogError("Error Model: {ErrorResponse}", errorModel);
        logger.LogError("Exception: {Exception}", exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        if (errorModel is not null)
        {
            var result = JsonConvert.SerializeObject(errorModel);

            return context.Response.WriteAsync(result);
        }
        else
        {
            return Task.CompletedTask;
        }
    }
}
