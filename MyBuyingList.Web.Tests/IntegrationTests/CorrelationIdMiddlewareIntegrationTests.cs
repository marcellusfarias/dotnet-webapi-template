using MyBuyingList.Web.Middlewares.CorrelationId;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net;

namespace MyBuyingList.Web.Tests.IntegrationTests;

public class CorrelationIdMiddlewareIntegrationTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly ResourceFactory _factory;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public CorrelationIdMiddlewareIntegrationTests(ResourceFactory resourceFactory)
        : base(resourceFactory)
    {
        _client = resourceFactory.HttpClient;
        _factory = resourceFactory;
    }

    [Fact]
    public async Task Request_WithoutCorrelationIdHeader_ResponseContainsGeneratedCorrelationId()
    {
        // Act
        var response = await _client.GetAsync("api/users", _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey(CorrelationIdMiddleware.HeaderName);
        response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).First().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Request_WithCorrelationIdHeader_ResponseEchoesTheSameValue()
    {
        // Arrange
        var correlationId = "test-correlation-abc123";

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/users");
        request.Headers.Add(CorrelationIdMiddleware.HeaderName, correlationId);
        request.Headers.Authorization = _client.DefaultRequestHeaders.Authorization;

        // Act
        var response = await _client.SendAsync(request, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).First().Should().Be(correlationId);
    }

    [Fact]
    public async Task Request_WithEmptyCorrelationIdHeader_ResponseContainsGeneratedId()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/users");
        request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, string.Empty);
        request.Headers.Authorization = _client.DefaultRequestHeaders.Authorization;

        // Act
        var response = await _client.SendAsync(request, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).First().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Request_WithCorrelationIdHeader_CorrelationIdAppearsInLogs()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        _factory.LogCollector.Clear();

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/users");
        request.Headers.Add(CorrelationIdMiddleware.HeaderName, correlationId);
        request.Headers.Authorization = _client.DefaultRequestHeaders.Authorization;

        // Act
        await _client.SendAsync(request, _cancellationToken);

        // Assert — the middleware logs "Request correlation ID: {CorrelationId}" inside the scope block.
        var entries = _factory.LogCollector.Entries;
        entries.Should().Contain(e =>
            e.Message.Contains("Request correlation ID:") &&
            e.Scopes.Any(s => s.Key == "CorrelationId" && s.Value != null && s.Value.ToString() == correlationId));
    }
}
