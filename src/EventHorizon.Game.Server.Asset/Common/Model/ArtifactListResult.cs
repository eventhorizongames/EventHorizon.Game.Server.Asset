namespace EventHorizon.Game.Server.Asset.Common.Model;

using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Common.Import.Model;

public class ArtifactListResult
{
    public IEnumerable<ExportArtifact> ExportList { get; internal set; } = new List<ExportArtifact>();
    public IEnumerable<ImportArtifact> ImportList { get; internal set; } = new List<ImportArtifact>();
    public IEnumerable<BackupArtifact> BackupList { get; internal set; } = new List<BackupArtifact>();
}
