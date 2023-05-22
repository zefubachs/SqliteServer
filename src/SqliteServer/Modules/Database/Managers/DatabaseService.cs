using SqliteServer.Modules.Database.Models;
using SqliteServer.Modules.Database.Validators;
using SqliteServer.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SqliteServer.Data.Configuration;

namespace SqliteServer.Modules.Database.Managers;

public class DatabaseService
{
    private readonly ApplicationDbContext context;
    private readonly StorageOptions storageOptions;
    private readonly ILogger<DatabaseService> logger;

    public DatabaseService(ApplicationDbContext context, IOptions<StorageOptions> storageOptions, ILogger<DatabaseService> logger)
    {
        this.context = context;
        this.storageOptions = storageOptions.Value;
        this.logger = logger;
    }

    public async Task<IReadOnlyCollection<DatabaseInfo>> ListDatabasesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Retrieving all databases");
        return await context.Databases.OrderBy(x => x.Name)
            .Select(x => new DatabaseInfo { Name = x.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<DatabaseInfo> CreateDatabaseAsync(CreateDatabaseRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new database {Request}", request);
        var validator = new CreateDatabaseValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = new Data.Application.Entities.Database { Name = request.Name! };
        context.Databases.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new DatabaseInfo { Name = entity.Name };
    }

    public async Task<DatabaseInfo> AttachDatabaseAsync(string name, IFormFile file, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attaching new database");
        var entity = new Data.Application.Entities.Database
        {
            Name = name,
        };

        var path = GetDatabaseFile(entity.Name);
        using (var stream = file.OpenReadStream())
        using (var fileStream = File.Create(path))
        {
            await stream.CopyToAsync(fileStream);
        }

        // TODO: Verify if file is a valid SQLite database file

        try
        {
            context.Databases.Add(entity);
            await context.SaveChangesAsync(cancellationToken);
            return new DatabaseInfo { Name = entity.Name };
        }
        catch (Exception)
        {
            try
            {

                File.Delete(path);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Unable to delete database file {Path}", path);
            }
            throw;
        }
    }

    public async Task<bool> DeleteDatabaseAsync(string name, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting database {Name}", name);
        var affectedRows = await context.Databases
            .Where(x => x.Name == name)
            .ExecuteDeleteAsync(cancellationToken);

        if (affectedRows == 0)
            return false;

        var path = GetDatabaseFile(name);
        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to delete database file {Path}", path);
        }
        return true;
    }

    private string GetDatabaseFile(string name) => Path.Combine(storageOptions.Data!, name + "sqlite");
}
