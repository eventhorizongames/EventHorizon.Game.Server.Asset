namespace EventHorizon.Game.Server.Asset.Hub.Base;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Backup.Query;
using EventHorizon.Game.Server.Asset.Common.Export.Query;
using EventHorizon.Game.Server.Asset.Common.Import.Query;
using EventHorizon.Game.Server.Asset.Common.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using Microsoft.AspNetCore.SignalR;

public partial class AdminHub
    : Hub
{
    public async Task<CommandResult<ArtifactListResult>> ArtifactList()
    {
        var result = new ArtifactListResult();

        var exportResult = await _sender.Send(
            new QueryForExportArtifactList(),
            Context.ConnectionAborted
        );
        if (!exportResult)
        {
            return exportResult.ErrorCode;
        }
        result.ExportList = exportResult.Result;

        var importResult = await _sender.Send(
            new QueryForImportArtifactList(),
            Context.ConnectionAborted
        );
        if (!importResult)
        {
            return importResult.ErrorCode;
        }
        result.ImportList = importResult.Result;

        var backupResult = await _sender.Send(
            new QueryForBackupArtifactList(),
            Context.ConnectionAborted
        );
        if (!backupResult)
        {
            return backupResult.ErrorCode;
        }
        result.BackupList = backupResult.Result;

        return result;
    }
}
