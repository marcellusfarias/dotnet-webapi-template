namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

[Collection(Constants.ResourceFactoryCollection)]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabaseAsync;

    public BaseIntegrationTest(ResourceFactory resourceFactory)
    {
        _resetDatabaseAsync = resourceFactory.ResetDatabaseAsync;
    }

    // This ensures that the database is reset before each test
    public async ValueTask InitializeAsync() => await _resetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
