using Microsoft.Extensions.DependencyInjection;
using MyBuyingList.Infrastructure;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;

namespace MyBuyingList.Web.Tests.IntegrationTests;

public class HealthIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly ResourceFactory _resourceFactory;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public HealthIntegrationTests(ResourceFactory resourceFactory)
        : base(resourceFactory)
    {
        _client = resourceFactory.CreateClient();
        _resourceFactory = resourceFactory;
    }

    [Fact]
    public async Task Health_ReturnsOk_WhenDatabaseIsReachable()
    {
        // Act
        var response = await _client.GetAsync(Constants.AddressHealthEndpoint, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync(_cancellationToken);
        body.Should().Be("Healthy");
    }

    [Fact]
    public async Task Health_IsAccessible_WithoutAuthentication()
    {
        // Act
        var response = await _client.GetAsync(Constants.AddressHealthEndpoint, _cancellationToken);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Health_ReturnsServiceUnavailable_WhenDatabaseIsDown()
    {
        // Arrange — drop the database to simulate it being unavailable
        using var scope = _resourceFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureDeletedAsync(_cancellationToken);

        // Act
        var response = await _client.GetAsync(Constants.AddressHealthEndpoint, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        var body = await response.Content.ReadAsStringAsync(_cancellationToken);
        body.Should().Be("Unhealthy");
    }
}
