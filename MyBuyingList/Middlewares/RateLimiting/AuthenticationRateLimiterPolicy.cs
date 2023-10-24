using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.RateLimiting;

namespace MyBuyingList.Web.Middlewares.RateLimiting;

public class AuthenticationRateLimiterPolicy : IRateLimiterPolicy<IPAddress>
{
    private Func<OnRejectedContext, CancellationToken, ValueTask>? _onRejected;
    private readonly CustomRateLimiterOptions _options;

    public AuthenticationRateLimiterPolicy(ILogger<AuthenticationRateLimiterPolicy> logger,
                                   IOptions<CustomRateLimiterOptions> options)
    {
        _onRejected = (ctx, token) =>
        {
            ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            logger.LogWarning($"Request rejected by {nameof(AuthenticationRateLimiterPolicy)}");
            return ValueTask.CompletedTask;
        };
        _options = options.Value;
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => _onRejected;

    public RateLimitPartition<IPAddress> GetPartition(HttpContext httpContext)
    {
        // can't be null since it's TCP by default.
        // check: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.connectioninfo.remoteipaddress?view=aspnetcore-7.0
        IPAddress ipAddress = httpContext.Connection.RemoteIpAddress!; 

        return RateLimitPartition.GetFixedWindowLimiter(ipAddress,
            _ =>
            {
                return new FixedWindowRateLimiterOptions
                {
                    PermitLimit = _options.PermitLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = _options.QueueLimit,
                    Window = TimeSpan.FromSeconds(_options.Window)
                };
            });
    }
}
