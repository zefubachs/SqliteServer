namespace SqliteServer.Modules.Database.Models;

public class QueryResponse
{
    public List<QueryResult> Results { get; set; } = new List<QueryResult>();
}
