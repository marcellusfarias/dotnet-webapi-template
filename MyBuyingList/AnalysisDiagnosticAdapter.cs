using Microsoft.Extensions.DiagnosticAdapter;

namespace MyBuyingList.Web;

// Created accordingly to https://andrewlock.net/understanding-your-middleware-pipeline-in-dotnet-6-with-the-middleware-analysis-package/
public class AnalysisDiagnosticAdapter
{
    private readonly ILogger<AnalysisDiagnosticAdapter> _logger;
    public AnalysisDiagnosticAdapter(ILogger<AnalysisDiagnosticAdapter> logger)
    {
        _logger = logger;
    }

    [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
    public void OnMiddlewareStarting(HttpContext httpContext, string name, Guid instance, long timestamp)
    {
        _logger.LogInformation("MiddlewareStarting: '{Name}'; Request Path: '{RequestPath}'", name, httpContext.Request.Path);
    }

    [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
    public void OnMiddlewareException(Exception exception, HttpContext httpContext, string name, Guid instance, long timestamp, long duration)
    {
        _logger.LogInformation("MiddlewareException: '{Name}'; '{ExceptionMessage}'", name, exception.Message);
    }

    [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
    public void OnMiddlewareFinished(HttpContext httpContext, string name, Guid instance, long timestamp, long duration)
    {
        _logger.LogInformation("MiddlewareFinished: '{Name}'; Status: '{ResponseStatusCode}'", name, httpContext.Response.StatusCode);
    }
}