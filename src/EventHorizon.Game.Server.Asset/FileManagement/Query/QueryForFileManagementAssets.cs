namespace EventHorizon.Game.Server.Asset.FileManagement.Query
{
    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.FileManagement.Model;

    using MediatR;

    public class QueryForFileManagementAssets
        : IRequest<CommandResult<FileManagementAssets>>
    {
    }
}
