namespace EventHorizon.Game.Server.Asset.Backup.ClientActions;
using MediatR;

public class ClientActionFinishedAssetBackupEvent
    : INotification
{
    public string ReferenceId { get; }
    public string BackupPath { get; }

    public ClientActionFinishedAssetBackupEvent(
        string referenceId,
        string exportPath
    )
    {
        ReferenceId = referenceId;
        BackupPath = exportPath;
    }
}
