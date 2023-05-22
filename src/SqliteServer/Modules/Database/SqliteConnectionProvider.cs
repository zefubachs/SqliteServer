using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SqliteServer.Data.Configuration;
using SqliteServer.Options;

namespace SqliteServer.Modules.Database;

public class SqliteConnectionProvider : ISqliteConnectionProvider
{
    private readonly ApplicationDbContext context;
    private readonly StorageOptions storageOptions;
    private readonly IMemoryCache cache;
    private readonly ILogger<SqliteConnectionProvider> logger;

    public SqliteConnectionProvider(ApplicationDbContext context, IOptions<StorageOptions> storageOptions,
        IMemoryCache cache, ILogger<SqliteConnectionProvider> logger)
    {
        this.context = context;
        this.storageOptions = storageOptions.Value;
        this.cache = cache;
        this.logger = logger;
    }

    public async Task<SqliteConnection?> GetConnectionAsync(string name, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting connectionstring for database {Name}", name);
        var connectionString = await cache.GetOrCreateAsync($"connection.{name}", async (entry) =>
        {
            logger.LogTrace("Cache miss, retrieving database {Name} for master", name);
            var db = await context.Databases.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
            if (db is null)
                return null;

            var path = Path.Combine(storageOptions.Data!, db.Name + ".sqlite");
            return new SqliteConnectionStringBuilder { DataSource = path }.ConnectionString;
        });
        if (connectionString is null)
        {
            logger.LogDebug("Database {Name} does not exist", name);
            return null;
        }

        return new SqliteConnection(connectionString);
    }
}
