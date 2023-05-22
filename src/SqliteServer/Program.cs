using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Serilog;
using SqliteServer;
using SqliteServer.Data.Application.Entities;
using SqliteServer.Data.Configuration;
using SqliteServer.Data.Tracing;
using SqliteServer.Infrastructure;
using SqliteServer.Options;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();

var variables = Variables.Create(builder.Configuration.GetSection("Variables"));
builder.Services.AddSingleton(variables);

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IPostConfigureOptions<StorageOptions>, StorageOptions.PostConfigureOptions>();
builder.Services.Configure<TracingOptions>(builder.Configuration.GetSection("Tracing"));

builder.Services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var databaseFile = Path.Combine(builder.Configuration["Storage:System"]!, "master.sqlite");
    var connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = variables.Translate(databaseFile),
    }.ConnectionString;
    options.UseSqlite(connectionString);
});
builder.Services.AddDbContext<TracingDbContext>(options =>
{
    var databaseFile = Path.Combine(builder.Configuration["Storage:System"]!, "trace.sqlite");
    var connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = variables.Translate(databaseFile),
    }.ConnectionString;
    options.UseSqlite(connectionString);
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
});

builder.RegisterModules();

var app = builder.Build();

await UpdateMasterDatabase();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapModules();
await app.RunAsync();

async Task UpdateMasterDatabase()
{
    using var scope = app.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    if (!await roleManager.Roles.AnyAsync())
    {
        var adminRole = new ApplicationRole { Name = "Administrators" };
        await roleManager.CreateAsync(adminRole);
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (!await userManager.Users.AnyAsync())
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin",
        };
        await userManager.CreateAsync(adminUser, "welcome");
        await userManager.AddToRoleAsync(adminUser, "Administrators");
    }
}