using SqliteServer;
using SqliteServer.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SqliteServer.Options;
using SqliteServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();

builder.Services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddDbContext<ConfigurationDbContext>(options =>
{
    var connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(builder.Configuration["Storage:Root"]!, "master.sqlite"),
    }.ConnectionString;
    options.UseSqlite(connectionString);
});

builder.RegisterModules();

var app = builder.Build();

UpdateMasterDatabase();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapModules();
app.Run();

void UpdateMasterDatabase()
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    context.Database.EnsureCreated();
}