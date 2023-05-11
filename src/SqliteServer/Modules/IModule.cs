namespace SqliteServer.Modules;

public interface IModule
{
    void ConfigureServices(IModuleBuilder module);
    void ConfigureEndpoints(IEndpointRouteBuilder endpoints);
}
