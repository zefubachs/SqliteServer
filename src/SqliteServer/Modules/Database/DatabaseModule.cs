using SqliteServer.Modules.Database.Models;
using SqliteServer.Modules.Database.Managers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SqliteServer.Modules.Database;

public class DatabaseModule : IModule
{
    public void ConfigureServices(IModuleBuilder module)
    {
        module.Services.AddMemoryCache();

        module.Services.AddTransient<ISqliteConnectionProvider, SqliteConnectionProvider>();

        module.Services.AddTransient<DatabaseService>();
        module.Services.AddTransient<CommandService>();
        module.Services.AddTransient<SessionStore>();
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("api/db", group =>
        {
            group.MapGet("/", ListDatabases).WithOpenApi();
            group.MapPost("/", CreateDatabase).WithOpenApi();
            group.MapPut("{name}", AttachDatabase).WithOpenApi();
            group.MapDelete("{name}", DeleteDatabase).WithOpenApi();

            group.MapGet("{name}/transactions", ListTransactions).WithOpenApi();
            group.MapPost("{name}/transactions", StartTransaction).WithOpenApi();
            group.MapPost("{name}/transactions/{id}/savepoints/{checkpoint}", CreateSavepoint).WithOpenApi();
            group.MapDelete("{name}/transactions/{id}/savepoints/{checkpoint}", ReleaseSavepoint).WithOpenApi();
            group.MapPut("{name}/transactions/{id}/commit", CommitTransaction).WithOpenApi();
            group.MapPut("{name}/transactions/{id}/rollback", RollbackTransaction).WithOpenApi();
            group.MapPut("{name}/transactions/{id}/rollback/{checkpoint}", RollbackSavePoint).WithOpenApi();

            group.MapPost("{name}/query", Query).WithOpenApi();
            group.MapPost("{name}/execute", Execute).WithOpenApi();
        });

        endpoints.MapGrpcService<Endpoints.CommandEndpoint>();
    }

    private static async Task<IResult> ListDatabases(DatabaseService databaseManager, CancellationToken cancellationToken)
    {
        var databases = await databaseManager.ListDatabasesAsync(cancellationToken);
        return Results.Ok(databases);
    }

    private static async Task<IResult> CreateDatabase(CreateDatabaseRequest request,
        DatabaseService databaseManager, CancellationToken cancellationToken)
    {
        var response = await databaseManager.CreateDatabaseAsync(request, cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> AttachDatabase(string name, [FromForm] IFormFile file,
        DatabaseService databaseManager, CancellationToken cancellationToken)
    {
        var response = await databaseManager.AttachDatabaseAsync(name, file, cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteDatabase(string name,
        DatabaseService databaseManager, CancellationToken cancellationToken)
    {
        var response = await databaseManager.DeleteDatabaseAsync(name, cancellationToken);
        return response ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> ListTransactions(string name,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> StartTransaction(string name,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> CreateSavepoint(string name, string id,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> ReleaseSavepoint(string name, string id,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> CommitTransaction(string name, string id,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> RollbackTransaction(string name, string id,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> RollbackSavePoint(string name, string id, string checkpoint,
        CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> Query(string name, List<CommandRequest> commands,
        CommandService commandService, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> Execute(string name, string? transaction, List<CommandRequest> commands,
        CommandService commandService, CancellationToken cancellationToken)
    {
        await commandService.ExecuteAsync(name, commands, transaction, cancellationToken);
        return Results.Ok();
    }
}
