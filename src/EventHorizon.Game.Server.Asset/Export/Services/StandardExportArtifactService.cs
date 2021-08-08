namespace EventHorizon.Game.Server.Asset.Export.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Model;

    using Microsoft.AspNetCore.Hosting;

    internal class StandardExportArtifactService
        : ExportArtifactService
    {
        private readonly string _artifactPath;

        public StandardExportArtifactService(
            IWebHostEnvironment environment
        )
        {
            _artifactPath = Path.Combine(
                environment.ContentRootPath,
                "Exports"
            );
        }
        public IEnumerable<ExportArtifact> ArtifactList()
        {
            return Directory.GetFiles(
                _artifactPath
            ).Select(
                fullName => Path.GetFileName(
                    fullName
                )
            ).Select(
                fileName => $"/Exports/{fileName}"
            ).Select(
                path => new ExportArtifact()
                {
                    Path = path,
                }
            );
        }
    }
}
