namespace EventHorizon.Game.Server.Asset.Export.Api
{
    using System.Collections.Generic;

    using EventHorizon.Game.Server.Asset.Export.Model;

    public interface ExportArtifactService
    {
        IEnumerable<ExportArtifact> ArtifactList();
    }
}
