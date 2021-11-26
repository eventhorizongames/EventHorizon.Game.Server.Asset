namespace EventHorizon.Game.Server.Asset.Common.Import.Api;
using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Import.Model;

public interface ImportArtifactService
{
    IEnumerable<ImportArtifact> ArtifactList();
}
