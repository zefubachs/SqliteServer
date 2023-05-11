using Microsoft.Data.Sqlite;
using System.Collections.Concurrent;

namespace SqliteServer.Modules.Database.Managers;

public class SessionStore
{
    private readonly object sync = new();
    private readonly Dictionary<string, Session> entries = new();

    public Session? Get(string id)
    {
        lock (sync)
        {
            if (entries.TryGetValue(id, out var session))
                return session;
        }

        return null;
    }

    public bool Register(string id, Session session)
    {
        lock (sync)
        {
            return entries.TryAdd(id, session);
        }
    }

    public bool Remove(string id)
    {
        lock (sync)
        {
            return entries.Remove(id);
        }
    }

    public void Purge(TimeSpan expirationGrace)
    {
        lock (sync)
        {
            var expiredEntries = entries.Where(x => x.Value.IsExpired(expirationGrace));
            foreach (var entry in expiredEntries)
            {
                entries.Remove(entry.Key);
            }
        }
    }
}
