namespace EventHorizon.Game.Server.Asset.Hub.Base
{
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Admin.Model;
    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.Export.Model;
    using EventHorizon.Game.Server.Asset.Export.Query;
    using EventHorizon.Game.Server.Asset.Export.Trigger;

    using Microsoft.AspNetCore.SignalR;

    public partial class AdminHub
        : Hub
    {
        public async Task<ApiResponse<ExportStatus>> Export_Status()
        {
            return new ApiResponse<ExportStatus>
            {
                Success = true,
                Result = await _sender.Send(
                    new QueryForExportStatus(),
                    Context.ConnectionAborted
                ),
            };
        }

        public async Task<CommandResult<ExportTriggerResult>> Export_Trigger()
        {
            return await _sender.Send(
                new TriggerExportCommand(),
                Context.ConnectionAborted
            );
        }
    }
}
