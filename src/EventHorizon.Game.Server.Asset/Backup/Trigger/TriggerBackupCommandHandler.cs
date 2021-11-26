namespace EventHorizon.Game.Server.Asset.Backup.Trigger;

using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Backup.Api;
using EventHorizon.Game.Server.Asset.Backup.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public class TriggerBackupCommandHandler
    : IRequestHandler<TriggerBackupCommand, CommandResult<BackupTriggerResult>>
{
    private readonly BackupTriggerService _service;

    public TriggerBackupCommandHandler(
        BackupTriggerService service
    )
    {
        _service = service;
    }

    public async Task<CommandResult<BackupTriggerResult>> Handle(
        TriggerBackupCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _service.Trigger(
            cancellationToken
        );

        return new(
            result
        );
    }
}
