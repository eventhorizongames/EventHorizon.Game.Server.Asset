namespace EventHorizon.Game.Server.Asset.Hub.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Common.Export.Query;

using Microsoft.AspNetCore.SignalR;

public partial class AdminHub
    : Hub
{
    public async Task<CommandResult<IEnumerable<ExportArtifact>>> Export_ArtifactList()
    {
        return await _sender.Send(
            new QueryForExportArtifactList(),
            Context.ConnectionAborted
        );
    }
}
