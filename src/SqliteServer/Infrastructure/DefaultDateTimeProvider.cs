namespace SqliteServer.Infrastructure;

public class DefaultDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
