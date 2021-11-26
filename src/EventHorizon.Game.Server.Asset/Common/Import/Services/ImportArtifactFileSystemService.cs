namespace EventHorizon.Game.Server.Asset.Common.Import.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using EventHorizon.Game.Server.Asset.Common.Base;
using EventHorizon.Game.Server.Asset.Common.Import.Api;
using EventHorizon.Game.Server.Asset.Common.Import.Model;
using EventHorizon.Game.Server.Asset.Core.Api;

using Microsoft.AspNetCore.Hosting;

internal class ImportArtifactFileSystemService
    : ArtifactFileSystemServiceBase,
    ImportArtifactService
{
    public ImportArtifactFileSystemService(
        IWebHostEnvironment environment,
        AssetServerSettings settings
    ) : base(
        Path.Combine(
            environment.ContentRootPath,
            settings.DataDirectory,
            settings.ImportsDirectory
        ),
        settings.ImportsDirectory
    )
    { }

    public IEnumerable<ImportArtifact> ArtifactList()
        => ReadArtifactPath()
        .Select(
            artifact => new ImportArtifact
            {
                Service = artifact.Service,
                Path = artifact.Path,
            }
        );
}
