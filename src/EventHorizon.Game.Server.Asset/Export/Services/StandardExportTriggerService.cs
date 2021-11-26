namespace EventHorizon.Game.Server.Asset.Export.Services;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.BackgroundTasks.Queue;
using EventHorizon.Game.Server.Asset.Export.Api;
using EventHorizon.Game.Server.Asset.Export.Tasks;
using EventHorizon.Game.Server.Asset.Export.Model;

using MediatR;

internal class StandardExportTriggerService
    : ExportTriggerService
{
    private readonly ISender _sender;

    public StandardExportTriggerService(ISender sender)
    {
        _sender = sender;
    }

    public async Task<ExportTriggerResult> Trigger(
        CancellationToken cancellationToken
    )
    {
        var task = new ExportAssetsTask();

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
