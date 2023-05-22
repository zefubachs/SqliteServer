namespace SqliteServer;

public interface ITraceClient
{
    void Append(string database, Guid user, string command, string parameters);
}
