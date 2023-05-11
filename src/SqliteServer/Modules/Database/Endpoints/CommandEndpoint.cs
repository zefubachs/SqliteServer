namespace SqliteServer.Modules.Database.Endpoints;

using SqliteServer.Contracts.Commands;
using Grpc.Core;
using System.Threading.Tasks;

public class CommandEndpoint : CommandService.CommandServiceBase
{
    public override Task<CommandResponse> Execute(CommandRequest request, ServerCallContext context)
    {
        return base.Execute(request, context);
    }
}
