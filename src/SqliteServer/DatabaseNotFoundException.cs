namespace SqliteServer;

public class DatabaseNotFoundException : Exception
{
    public string Database { get; }

    public DatabaseNotFoundException(string database)
        : base($"Database '{database}' is not found.")
    {
        Database = database;
    }
}
