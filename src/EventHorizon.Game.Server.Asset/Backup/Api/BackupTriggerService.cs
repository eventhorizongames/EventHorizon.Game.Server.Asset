namespace EventHorizon.Game.Server.Asset.Backup.Api;

using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Backup.Model;

public interface BackupTriggerService
{
    Task<BackupTriggerResult> Trigger(
        CancellationToken cancellationToken
    );
}
