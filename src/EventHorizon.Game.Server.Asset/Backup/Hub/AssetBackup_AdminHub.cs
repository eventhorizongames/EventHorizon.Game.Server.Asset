namespace EventHorizon.Game.Server.Asset.Hub.Base;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Backup.Model;
using EventHorizon.Game.Server.Asset.Backup.Trigger;
using EventHorizon.Game.Server.Asset.Core.Command;

using Microsoft.AspNetCore.SignalR;

public partial class AdminHub
    : Hub
{
    public async Task<CommandResult<BackupTriggerResult>> Asset_Backup_Trigger()
    {
        return await _sender.Send(
            new TriggerBackupCommand(),
            Context.ConnectionAborted
        );
    }
}
