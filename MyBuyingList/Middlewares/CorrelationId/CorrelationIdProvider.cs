namespace MyBuyingList.Web.Middlewares.CorrelationId;

public sealed class CorrelationIdProvider : ICorrelationIdProvider
{
    public string CorrelationId { get; set; } = string.Empty;
}
