using Microsoft.Extensions.Logging;

namespace MyBuyingList.Web.Tests.IntegrationTests.Common.Logging;

public record FakeLogEntry(LogLevel Level, string Message, IReadOnlyList<KeyValuePair<string, object?>> Scopes);

public class FakeLogCollector
{
    private readonly List<FakeLogEntry> _entries = [];
    private readonly object _lock = new();

    public IReadOnlyList<FakeLogEntry> Entries
    {
        get { lock (_lock) { return _entries.ToList(); } }
    }

    public void Add(FakeLogEntry entry)
    {
        lock (_lock) { _entries.Add(entry); }
    }

    public void Clear()
    {
        lock (_lock) { _entries.Clear(); }
    }
}
