namespace SqliteServer.Data.Application.Entities;

public class Database
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}
