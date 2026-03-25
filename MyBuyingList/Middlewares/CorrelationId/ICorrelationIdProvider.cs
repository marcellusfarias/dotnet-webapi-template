namespace MyBuyingList.Web.Middlewares.CorrelationId;

public interface ICorrelationIdProvider
{
    string CorrelationId { get; }
}
