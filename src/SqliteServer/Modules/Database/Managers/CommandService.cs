using SqliteServer.Modules.Database.Models;

namespace SqliteServer.Modules.Database.Managers;

public class CommandService
{
    private readonly ISqliteConnectionProvider connectionProvider;
    private readonly SessionStore sessionStore;
    private readonly ILogger<CommandService> logger;

    public CommandService(ISqliteConnectionProvider connectionProvider, SessionStore sessionStore, ILogger<CommandService> logger)
    {
        this.connectionProvider = connectionProvider;
        this.sessionStore = sessionStore;
        this.logger = logger;
    }

    public async Task<ExecuteResponse> ExecuteAsync(string name, IEnumerable<CommandRequest> commands, string? transaction = null, CancellationToken cancellationToken = default)
    {
        var executor = await GetExecutorAsync(name, transaction);
        return await executor.ExecuteAsync(commands, cancellationToken);
    }

    public async Task<QueryResponse> QueryAsync(string name, IEnumerable<CommandRequest> commands, string? transaction = null, CancellationToken cancellationToken = default)
    {
        var executor = await GetExecutorAsync(name, transaction);
        return await executor.QueryAsync(commands, cancellationToken);
    }

    private async Task<CommandExecutor> GetExecutorAsync(string name, string? transaction)
    {
        if (transaction is null)
        {
            var connection = await connectionProvider.GetConnectionAsync(name);
            if (connection is null)
                throw new ArgumentException($"Database '{name}' does not exist", nameof(name));

            return new CommandExecutor(connection, null, true);
        }
        else
        {
            var session = sessionStore.Get(transaction);
            if (session is null)
                throw new ArgumentException($"Session '{transaction}' does not exist", nameof(transaction));

            return session.CreateExecutor();
        }
    }
}
