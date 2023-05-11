using Microsoft.Data.Sqlite;

namespace SqliteServer.Modules.Database.Managers;

public class Session
{
    private readonly SqliteConnection connection;
    private readonly SqliteTransaction transaction;
    private readonly IDateTimeProvider dateTimeProvider;

    public DateTime LastActivity { get; private set; }

    public Session(SqliteConnection connection, SqliteTransaction transaction, IDateTimeProvider dateTimeProvider)
    {
        this.connection = connection;
        this.transaction = transaction;
        this.dateTimeProvider = dateTimeProvider;

        LastActivity = dateTimeProvider.Now;
    }

    public bool IsExpired(TimeSpan expirationGrace)
    {
        return dateTimeProvider.Now > LastActivity.Add(expirationGrace);
    }

    public CommandExecutor CreateExecutor()
    {
        return new CommandExecutor(connection, transaction, false);
    }
}
