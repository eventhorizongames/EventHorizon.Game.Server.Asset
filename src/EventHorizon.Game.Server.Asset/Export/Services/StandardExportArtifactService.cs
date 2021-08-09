namespace EventHorizon.Game.Server.Asset.Export.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Model;

    using Microsoft.AspNetCore.Hosting;

    internal class StandardExportArtifactService
        : ExportArtifactService
    {
        private readonly string _artifactPath;
        private readonly AssetServerContentDirectories _directories;

        public StandardExportArtifactService(
            IWebHostEnvironment environment,
            AssetServerContentDirectories directories
        )
        {
            _artifactPath = Path.Combine(
                environment.ContentRootPath,
                directories.DataDirectory,
                directories.ExportsDirectory
            );
            _directories = directories;
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
                fileName => $"/{_directories.ExportsDirectory}/{fileName}"
            ).Select(
                path => new ExportArtifact()
                {
                    Path = path,
                }
            );
        }
    }
}
