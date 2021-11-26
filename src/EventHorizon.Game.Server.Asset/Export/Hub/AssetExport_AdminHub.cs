namespace EventHorizon.Game.Server.Asset.Hub.Base;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Export.Model;
using EventHorizon.Game.Server.Asset.Export.Trigger;

using Microsoft.AspNetCore.SignalR;

public partial class AdminHub
    : Hub
{
    public async Task<CommandResult<ExportTriggerResult>> Asset_Export_Trigger()
    {
        return await _sender.Send(
            new TriggerExportCommand(),
            Context.ConnectionAborted
        );
    }
}
