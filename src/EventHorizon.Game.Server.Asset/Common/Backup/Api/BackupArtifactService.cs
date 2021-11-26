namespace EventHorizon.Game.Server.Asset.Common.Backup.Api;
using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Backup.Model;

public interface BackupArtifactService
{
    IEnumerable<BackupArtifact> ArtifactList();
}
