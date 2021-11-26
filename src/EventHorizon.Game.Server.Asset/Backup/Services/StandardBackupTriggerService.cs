namespace EventHorizon.Game.Server.Asset.Backup.Services;

using System.Threading;
using System.Threading.Tasks;

using EventHorizon.BackgroundTasks.Queue;
using EventHorizon.Game.Server.Asset.Backup.Api;
using EventHorizon.Game.Server.Asset.Backup.Model;
using EventHorizon.Game.Server.Asset.Backup.Tasks;

using MediatR;

internal class StandardBackupTriggerService
    : BackupTriggerService
{
    private readonly ISender _sender;

    public StandardBackupTriggerService(
        ISender sender
    )
    {
        _sender = sender;
    }

    public async Task<BackupTriggerResult> Trigger(
        CancellationToken cancellationToken
    )
    {
        var task = new BackupAssetsTask();

        await _sender.Send(
            new EnqueueBackgroundJob(
                task
            ),
            cancellationToken
        );

        return new()
        {
            ReferenceId = task.ReferenceId,
        };
    }
}
