namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

[Collection(Constants.ResourceFactoryCollection)]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabaseAsync;

    public BaseIntegrationTest(ResourceFactory resourceFactory)
    {
        _resetDatabaseAsync = resourceFactory.ResetDatabaseAsync;
    }

    public ValueTask InitializeAsync() => new(Task.CompletedTask);

    public async ValueTask DisposeAsync() => await _resetDatabaseAsync();
}
