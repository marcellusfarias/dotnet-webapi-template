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
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected

        switch (exception)
        {
            case CustomValidationException:
                logger.LogError($"Validation exception. Stack trace: {exception.InnerException}");
                code = HttpStatusCode.BadRequest; //400
                break;
            case AuthenticationException:
                logger.LogError(exception.Message);
                code = HttpStatusCode.Unauthorized; //401
                break;
            case ResourceNotFoundException:
                logger.LogError(exception.Message);
                code = HttpStatusCode.NotFound; // 404
                break;
            case BusinessLogicException:
                logger.LogError($"Business rule exception. Message: {exception.Message}");
                code = HttpStatusCode.UnprocessableEntity; //422
                break;
            case DatabaseException:
                logger.LogError($"Database exception. Stack trace: {exception.InnerException}");
                code = HttpStatusCode.InternalServerError; //500
                break;
        }

        var result = JsonConvert.SerializeObject(new { error = exception.Message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
