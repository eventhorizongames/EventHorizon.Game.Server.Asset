namespace EventHorizon.Game.Server.Asset.Common.Backup.Model;

public class RunUploadOfServiceBackupFileResult
{
    public string Service { get; }
    public string BackupFileFullName { get; }
    public string BackupPath { get; }

    public RunUploadOfServiceBackupFileResult(
        string service,
        string backupFileFullName,
        string backupPath
    )
    {
        Service = service;
        BackupFileFullName = backupFileFullName;
        BackupPath = backupPath;
    }
}
