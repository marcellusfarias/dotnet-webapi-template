namespace MyBuyingList.Web.Middlewares.RateLimiting;

public class CustomRateLimiterOptions
{
    public required int PermitLimit { get; init; }
    public required int QueueLimit { get; init; }
    public required int Window { get; init; }
}
