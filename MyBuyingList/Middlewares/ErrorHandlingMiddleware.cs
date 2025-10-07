using MyBuyingList.Application;
using MyBuyingList.Application.Common.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace MyBuyingList.Web.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        this.next = next;
        _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
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
        ErrorModel? errorModel = null;

        var formattedException = exception as IFormattedResponseException;
        if (formattedException is not null)
        {
            code = formattedException.StatusCode;
            errorModel = formattedException.Error;
        }

        logger.LogError($"StatusCode: {code}");
        logger.LogError($"Error Model: {errorModel}");
        logger.LogError($"Exception: {exception}");

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
