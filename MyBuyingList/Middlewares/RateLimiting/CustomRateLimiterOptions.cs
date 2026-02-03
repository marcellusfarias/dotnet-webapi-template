namespace MyBuyingList.Web.Middlewares.RateLimiting;

/// <summary>
/// Configuration options for rate limiting.
/// </summary>
public class CustomRateLimiterOptions
{
    /// <summary>Number of requests allowed per window.</summary>
    public required int PermitLimit { get; init; }

    /// <summary>Number of requests to queue when limit is reached.</summary>
    public required int QueueLimit { get; init; }

    /// <summary>Time window duration in seconds.</summary>
    public required int Window { get; init; }
}
