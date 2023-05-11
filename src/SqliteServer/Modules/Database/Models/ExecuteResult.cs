namespace SqliteServer.Modules.Database.Models;

public class ExecuteResult
{
    public TimeSpan Duration { get; init; }
    public int AffectedRows { get; init; }
}
