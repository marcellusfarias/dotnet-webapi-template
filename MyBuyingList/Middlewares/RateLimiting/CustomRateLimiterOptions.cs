namespace MyBuyingList.Web.Middlewares.RateLimiting;

public class CustomRateLimiterOptions
{
    public required int PermitLimit { get; set; }
    public required int QueueLimit { get; set; }
    public required int Window { get; set; }
}
