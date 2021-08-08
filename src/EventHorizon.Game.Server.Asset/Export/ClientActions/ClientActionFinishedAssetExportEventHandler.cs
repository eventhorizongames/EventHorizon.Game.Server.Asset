namespace EventHorizon.Game.Server.Asset.Export.ClientActions
{
    using System.Threading;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Hub.Base;

    using MediatR;

    using Microsoft.AspNetCore.SignalR;

    public class ClientActionFinishedAssetExportEventHandler
        : INotificationHandler<ClientActionFinishedAssetExportEvent>
    {
        private readonly IHubContext<AdminHub> _hubContext;

        public ClientActionFinishedAssetExportEventHandler(
            IHubContext<AdminHub> hubContext
        )
        {
            _hubContext = hubContext;
        }

        public async Task Handle(
            ClientActionFinishedAssetExportEvent notification,
            CancellationToken cancellationToken
        )
        {
            await _hubContext.Clients.All.SendAsync(
                "ExportFinished",
                notification.ReferenceId,
                notification.ExportPath,
                cancellationToken: cancellationToken
            );
        }
    }
}
