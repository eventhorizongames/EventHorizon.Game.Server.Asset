namespace EventHorizon.Game.Server.Asset.Backup.Tasks;
using System;

using EventHorizon.BackgroundTasks.Model;

public class BackupAssetsTask
    : BackgroundTask
{
    public string ReferenceId { get; } = Guid.NewGuid().ToString();
}
