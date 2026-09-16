namespace EventHotelBroker.Services;

/// <summary>
/// Singleton service that persists JWT tokens in memory across Blazor Server SignalR circuit reconnects.
/// Keyed by a stable browser-generated client ID stored in a cookie, not by circuit/session.
/// </summary>
public class TokenStore
{
    private readonly Dictionary<string, string> _tokens = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public void Set(string clientId, string token)
    {
        lock (_lock)
        {
            _tokens[clientId] = token;
        }
    }

    public string? Get(string clientId)
    {
        lock (_lock)
        {
            return _tokens.TryGetValue(clientId, out var t) ? t : null;
        }
    }

    public void Remove(string clientId)
    {
        lock (_lock)
        {
            _tokens.Remove(clientId);
        }
    }
}
