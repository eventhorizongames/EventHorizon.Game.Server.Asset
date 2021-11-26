namespace EventHorizon.Game.Server.Asset.Common.Export.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using EventHorizon.Game.Server.Asset.Common.Base;
using EventHorizon.Game.Server.Asset.Common.Export.Api;
using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Core.Api;

using Microsoft.AspNetCore.Hosting;

internal class ExportArtifactFileSystemService
    : ArtifactFileSystemServiceBase,
    ExportArtifactService
{
    public ExportArtifactFileSystemService(
        IWebHostEnvironment environment,
        AssetServerSettings settings
    ) : base(
        Path.Combine(
            environment.ContentRootPath,
            settings.DataDirectory,
            settings.ExportsDirectory
        ),
        settings.ExportsDirectory
    )
    { }

    public IEnumerable<ExportArtifact> ArtifactList()
        => ReadArtifactPath()
        .Select(
            artifact => new ExportArtifact
            {
                Service = artifact.Service,
                Path = artifact.Path,
            }
        );
}
