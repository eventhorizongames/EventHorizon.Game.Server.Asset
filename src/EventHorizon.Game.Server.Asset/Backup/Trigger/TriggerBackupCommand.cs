namespace EventHorizon.Game.Server.Asset.Backup.Trigger;

using EventHorizon.Game.Server.Asset.Backup.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public struct TriggerBackupCommand
    : IRequest<CommandResult<BackupTriggerResult>>
{
}
