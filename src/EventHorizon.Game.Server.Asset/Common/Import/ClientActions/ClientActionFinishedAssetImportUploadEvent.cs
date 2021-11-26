namespace EventHorizon.Game.Server.Asset.Common.Import.ClientActions;
using MediatR;

public struct ClientActionFinishedAssetImportUploadEvent
    : INotification
{
    public string Service { get; }
    public string ImportPath { get; }

    public ClientActionFinishedAssetImportUploadEvent(
        string service,
        string importPath
    )
    {
        Service = service;
        ImportPath = importPath;
    }
}
