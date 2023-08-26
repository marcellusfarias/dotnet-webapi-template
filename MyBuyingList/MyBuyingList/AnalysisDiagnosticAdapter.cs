using Microsoft.Extensions.DiagnosticAdapter;

namespace MyBuyingList.Web
{
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
            _logger.LogInformation($"MiddlewareStarting: '{name}'; Request Path: '{httpContext.Request.Path}'");
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
        public void OnMiddlewareException(Exception exception, HttpContext httpContext, string name, Guid instance, long timestamp, long duration)
        {
            _logger.LogInformation($"MiddlewareException: '{name}'; '{exception.Message}'");
        }

        [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
        public void OnMiddlewareFinished(HttpContext httpContext, string name, Guid instance, long timestamp, long duration)
        {
            _logger.LogInformation($"MiddlewareFinished: '{name}'; Status: '{httpContext.Response.StatusCode}'");
        }
    }
}
