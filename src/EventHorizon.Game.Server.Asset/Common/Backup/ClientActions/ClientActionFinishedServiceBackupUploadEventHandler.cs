namespace EventHorizon.Game.Server.Asset.Common.Backup.ClientActions;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Hub.Base;

using MediatR;

using Microsoft.AspNetCore.SignalR;

public class ClientActionFinishedServiceBackupUploadEventHandler
    : INotificationHandler<ClientActionFinishedServiceBackupUploadEvent>
{
    private readonly IHubContext<AdminHub> _hubContext;

    public ClientActionFinishedServiceBackupUploadEventHandler(
        IHubContext<AdminHub> hubContext
    )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(
        ClientActionFinishedServiceBackupUploadEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _hubContext.Clients.All.SendAsync(
            "BackupUploadFinished",
            notification.Service,
            notification.BackupPath,
            cancellationToken: cancellationToken
        );
    }
}
