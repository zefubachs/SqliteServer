using Microsoft.Extensions.Options;

namespace SqliteServer.Options;

public class StorageOptions
{
    public string System { get; set; } = null!;
    public string Data { get; set; } = null!;

    public class PostConfigureOptions : IPostConfigureOptions<StorageOptions>
    {
        private readonly Variables variables;

        public PostConfigureOptions(Variables variables)
        {
            this.variables = variables;
        }

        public void PostConfigure(string? name, StorageOptions options)
        {
            options.Data = variables.Translate(options.Data);
        }
    }
}
