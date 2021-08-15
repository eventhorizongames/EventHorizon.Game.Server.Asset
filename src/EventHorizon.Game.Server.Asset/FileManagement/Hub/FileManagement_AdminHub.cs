namespace EventHorizon.Game.Server.Asset.Hub.Base
{
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.FileManagement.Model;
    using EventHorizon.Game.Server.Asset.FileManagement.Query;

    using Microsoft.AspNetCore.SignalR;

    public partial class AdminHub
        : Hub
    {
        public async Task<CommandResult<FileManagementAssets>> FileManagement_Assets()
        {
            return await _sender.Send(
                new QueryForFileManagementAssets(),
                Context.ConnectionAborted
            );
        }
    }
}
