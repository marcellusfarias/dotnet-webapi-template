namespace MyBuyingList.Web.Middlewares.CorrelationId;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, CorrelationIdProvider correlationIdProvider)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId) || !IsValid(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        correlationIdProvider.CorrelationId = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context);
        }
    }

    // Valid: printable ASCII (0x20–0x7E), max 128 characters.
    // Anything else (control chars, newlines, non-ASCII) is rejected and a new ID is generated.
    private static bool IsValid(string value)
    {
        if (value.Length > 128)
            return false;

        foreach (char c in value)
        {
            if (c < 0x20 || c > 0x7E)
                return false;
        }

        return true;
    }
}
