namespace SqliteServer;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}
