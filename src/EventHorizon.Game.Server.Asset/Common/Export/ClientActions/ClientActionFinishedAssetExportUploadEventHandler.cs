namespace EventHorizon.Game.Server.Asset.Common.Export.ClientActions;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Hub.Base;

using MediatR;

using Microsoft.AspNetCore.SignalR;

public class ClientActionFinishedAssetExportUploadEventHandler
    : INotificationHandler<ClientActionFinishedServiceExportUploadEvent>
{
    private readonly IHubContext<AdminHub> _hubContext;

    public ClientActionFinishedAssetExportUploadEventHandler(
        IHubContext<AdminHub> hubContext
    )
    {
        _hubContext = hubContext;
    }

    public async Task Handle(
        ClientActionFinishedServiceExportUploadEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _hubContext.Clients.All.SendAsync(
            "ExportUploadFinished",
            notification.Service,
            notification.ExportPath,
            cancellationToken: cancellationToken
        );
    }
}
