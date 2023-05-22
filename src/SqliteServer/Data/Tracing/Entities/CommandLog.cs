namespace SqliteServer.Data.Tracing.Entities;

public class CommandLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid User { get; set; }
    public required string Database { get; set; }
    public required string Command { get; set; }
    public required string Parameters { get; set; }
}
