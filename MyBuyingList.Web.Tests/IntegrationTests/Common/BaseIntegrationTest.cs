namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

[Collection(Constants.ResourceFactoryCollection)]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabaseAsync;

    public BaseIntegrationTest(ResourceFactory resourceFactory)
    {
        _resetDatabaseAsync = resourceFactory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabaseAsync();
}
