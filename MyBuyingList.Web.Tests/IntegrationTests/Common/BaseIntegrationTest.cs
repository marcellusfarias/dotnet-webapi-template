﻿namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

[Collection(Constants.ResourceFactoryCollection)]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly Func<Task> ResetDatabaseAsync;

    public BaseIntegrationTest(ResourceFactory resourceFactory)
    {
        ResetDatabaseAsync = resourceFactory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await ResetDatabaseAsync();
}
