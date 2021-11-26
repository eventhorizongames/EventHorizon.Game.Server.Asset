namespace EventHorizon.Game.Server.Asset.Common.Export.ClientActions;
using MediatR;

public struct ClientActionFinishedServiceExportUploadEvent
    : INotification
{
    public string Service { get; }
    public string ExportPath { get; }

    public ClientActionFinishedServiceExportUploadEvent(
        string service,
        string exportPath
    )
    {
        Service = service;
        ExportPath = exportPath;
    }
}
