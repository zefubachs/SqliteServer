namespace SqliteServer.Modules;

public interface IModuleBuilder
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
    IWebHostEnvironment Environment { get; }
}
