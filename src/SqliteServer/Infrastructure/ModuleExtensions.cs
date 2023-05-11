using SqliteServer.Modules;

namespace SqliteServer;

public static class ModuleExtensions
{
    private static readonly List<IModule> registeredModules = new List<IModule>();

    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder app)
    {
        var builder = new ModuleBuilder(app.Services, app.Configuration, app.Environment);
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            module.ConfigureServices(builder);
            registeredModules.Add(module);
        }
        return app;
    }

    public static WebApplication MapModules(this WebApplication app)
    {
        foreach (var module in registeredModules)
        {
            module.ConfigureEndpoints(app);
        }
        return app;
    }

    public static IEndpointRouteBuilder MapGroup(this IEndpointRouteBuilder endpoints, string prefix, Action<IEndpointRouteBuilder> groupBuilder)
    {
        var group = endpoints.MapGroup(prefix);
        groupBuilder(group);
        return endpoints;
    }

    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(x => x.IsClass && typeof(IModule).IsAssignableFrom(x))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }

    private sealed class ModuleBuilder : IModuleBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public ModuleBuilder(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            Services = services;
            Configuration = configuration;
            Environment = environment;
        }
    }
}
