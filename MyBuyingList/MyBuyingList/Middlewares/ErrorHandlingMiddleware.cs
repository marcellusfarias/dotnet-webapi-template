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

    public async Task Invoke(HttpContext context /* other dependencies */)
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

        int code = (int) HttpStatusCode.InternalServerError; // 500 if unhandled
        string message = exception.Message;
        string contentType = "application/json";

        var customException = exception as ICustomHttpException;
        if (customException is not null)
        {
            message = customException.HttpResponseMessage;
            code = customException.HttpResponseCode;
        }

        logger.LogError(message);
        logger.LogError(exception.ToString());

        context.Response.ContentType = contentType;
        context.Response.StatusCode = code;
        var result = JsonConvert.SerializeObject(new { error = message });
        return context.Response.WriteAsync(result);
    }
}
