using System.Text.Json;
using System.Text.Json.Nodes;

namespace SqliteServer.Modules.Database.Models;

public class CommandRequest
{
    public required string Command { get; set; }
    public JsonElement Parameters { get; set; }
}
