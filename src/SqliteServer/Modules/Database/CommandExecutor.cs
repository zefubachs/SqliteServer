using Microsoft.Data.Sqlite;
using SqliteServer.Modules.Database.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SqliteServer.Modules.Database;

public class CommandExecutor : IDisposable
{
    private SqliteConnection connection;
    private SqliteTransaction? transaction;

    private bool ownsConnection;
    private bool disposedValue;

    public CommandExecutor(SqliteConnection connection, SqliteTransaction? transaction, bool ownsConnection)
    {
        this.connection = connection;
        this.transaction = transaction;
        this.ownsConnection = ownsConnection;
    }

    public async Task<ExecuteResponse> ExecuteAsync(IEnumerable<CommandRequest> requests, CancellationToken cancellationToken = default)
    {
        var command = new SqliteCommand(null, connection, transaction);
        var response = new ExecuteResponse();
        var stopwatch = new Stopwatch();
        foreach (var request in requests)
        {
            PrepareCommand(command, request);

            stopwatch.Start();
            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            stopwatch.Stop();
            stopwatch.Reset();

            response.Results.Add(new ExecuteResult
            {
                AffectedRows = affectedRows,
                Duration = stopwatch.Elapsed,
            });
        }
        return response;
    }

    public async Task<QueryResponse> QueryAsync(IEnumerable<CommandRequest> requests, CancellationToken cancellationToken = default)
    {
        var command = new SqliteCommand(null, connection, transaction);
        var response = new QueryResponse();
        var stopwatch = new Stopwatch();
        foreach (var request in requests)
        {

        }
        return response;
    }

    private static void PrepareCommand(SqliteCommand command, CommandRequest request)
    {
        command.CommandText = request.Command;
        command.Parameters.Clear();
        switch (request.Parameters.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in request.Parameters.EnumerateObject())
                {
                    command.Parameters.AddWithValue(property.Name, property.Value.ValueKind switch
                    {
                        JsonValueKind.Number => property.Value.GetInt32(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => property.Value.GetString(),
                    });
                }
                break;
            case JsonValueKind.Array:
                foreach (var item in request.Parameters.EnumerateArray())
                {

                }
                break;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing && ownsConnection)
            {
                transaction?.Dispose();
                connection.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
