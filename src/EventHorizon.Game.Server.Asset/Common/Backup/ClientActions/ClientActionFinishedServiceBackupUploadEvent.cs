namespace EventHorizon.Game.Server.Asset.Common.Backup.ClientActions;
using MediatR;

public struct ClientActionFinishedServiceBackupUploadEvent
    : INotification
{
    public string Service { get; }
    public string BackupPath { get; }

    public ClientActionFinishedServiceBackupUploadEvent(
        string service,
        string exportPath
    )
    {
        Service = service;
        BackupPath = exportPath;
    }
}
