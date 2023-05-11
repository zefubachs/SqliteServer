using Microsoft.Data.Sqlite;

namespace SqliteServer.Modules.Database;

public interface ISqliteConnectionProvider
{
    Task<SqliteConnection?> GetConnectionAsync(string name, CancellationToken cancellationToken = default);
}
