namespace EventHorizon.Game.Server.Asset.Common.Import.ClientActions;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Hub.Base;

using MediatR;

using Microsoft.AspNetCore.SignalR;

public class ClientActionFinishedServiceImportUploadEventHandler
    : INotificationHandler<ClientActionFinishedAssetImportUploadEvent>
{
    private readonly IHubContext<AdminHub> _hubContext;

    public ClientActionFinishedServiceImportUploadEventHandler(
        IHubContext<AdminHub> hubContext
    )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(
        ClientActionFinishedAssetImportUploadEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _hubContext.Clients.All.SendAsync(
            "ImportUploadFinished",
            notification.Service,
            notification.ImportPath,
            cancellationToken: cancellationToken
        );
    }
}
