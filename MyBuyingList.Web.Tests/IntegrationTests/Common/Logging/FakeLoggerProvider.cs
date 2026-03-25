using Microsoft.Extensions.Logging;

namespace MyBuyingList.Web.Tests.IntegrationTests.Common.Logging;

public class FakeLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly FakeLogCollector _collector;
    internal IExternalScopeProvider ScopeProvider { get; private set; } = new LoggerExternalScopeProvider();

    public FakeLoggerProvider(FakeLogCollector collector)
    {
        _collector = collector;
    }

    public ILogger CreateLogger(string categoryName) => new FakeLogger(_collector, this);

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        ScopeProvider = scopeProvider;
    }

    public void Dispose() { }
}

internal sealed class FakeLogger : ILogger
{
    private readonly FakeLogCollector _collector;
    private readonly FakeLoggerProvider _provider;

    public FakeLogger(FakeLogCollector collector, FakeLoggerProvider provider)
    {
        _collector = collector;
        _provider = provider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => _provider.ScopeProvider.Push(state);

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var scopes = new List<KeyValuePair<string, object?>>();

        _provider.ScopeProvider.ForEachScope((scopeState, list) =>
        {
            if (scopeState is IEnumerable<KeyValuePair<string, object>> kvps)
            {
                foreach (var kvp in kvps)
                {
                    list.Add(new KeyValuePair<string, object?>(kvp.Key, kvp.Value));
                }
            }
        }, scopes);

        _collector.Add(new FakeLogEntry(logLevel, formatter(state, exception), scopes.AsReadOnly()));
    }
}
