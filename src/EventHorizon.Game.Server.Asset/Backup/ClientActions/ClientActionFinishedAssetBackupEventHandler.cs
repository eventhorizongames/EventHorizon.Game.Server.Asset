namespace EventHorizon.Game.Server.Asset.Backup.ClientActions;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Hub.Base;

using MediatR;

using Microsoft.AspNetCore.SignalR;

public class ClientActionFinishedAssetBackupEventHandler
    : INotificationHandler<ClientActionFinishedAssetBackupEvent>
{
    private readonly IHubContext<AdminHub> _hubContext;

    public ClientActionFinishedAssetBackupEventHandler(
        IHubContext<AdminHub> hubContext
    )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(
        ClientActionFinishedAssetBackupEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _hubContext.Clients.All.SendAsync(
            "AssetBackupFinished",
            notification.ReferenceId,
            notification.BackupPath,
            cancellationToken: cancellationToken
        );
    }
}
