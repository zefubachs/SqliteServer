namespace SqliteServer.Modules.Database.Models;

public class ExecuteResponse
{
    public List<ExecuteResult> Results { get; } = new List<ExecuteResult>();
}
