namespace EventHorizon.Game.Server.Asset.Export.Tasks;
using System;

using EventHorizon.BackgroundTasks.Model;

public class ExportAssetsTask
    : BackgroundTask
{
    public string ReferenceId { get; } = Guid.NewGuid().ToString();
}
