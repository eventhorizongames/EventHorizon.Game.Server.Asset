namespace EventHorizon.Game.Server.Asset.Common.Export.Api;
using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Export.Model;

public interface ExportArtifactService
{
    IEnumerable<ExportArtifact> ArtifactList();
}
