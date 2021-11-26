namespace EventHorizon.Game.Server.Asset.Export.ClientActions;
using MediatR;

public struct ClientActionFinishedAssetExportEvent
    : INotification
{
    public string ReferenceId { get; }
    public string ExportPath { get; }

    public ClientActionFinishedAssetExportEvent(
        string referenceId,
        string exportPath
    )
    {
        ReferenceId = referenceId;
        ExportPath = exportPath;
    }
}
