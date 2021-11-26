namespace EventHorizon.Game.Server.Asset.Common.Backup.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using EventHorizon.Game.Server.Asset.Common.Backup.Api;
using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Common.Base;
using EventHorizon.Game.Server.Asset.Core.Api;

using Microsoft.AspNetCore.Hosting;

internal class BackupArtifactFileSystemService
    : ArtifactFileSystemServiceBase,
    BackupArtifactService
{
    public BackupArtifactFileSystemService(
        IWebHostEnvironment environment,
        AssetServerSettings settings
    ) : base(
        Path.Combine(
            environment.ContentRootPath,
            settings.DataDirectory,
            settings.BackupsDirectory
        ),
        settings.BackupsDirectory
    )
    { }

    public IEnumerable<BackupArtifact> ArtifactList()
        => ReadArtifactPath()
        .Select(
            artifact => new BackupArtifact
            {
                Service = artifact.Service,
                Path = artifact.Path,
            }
        );
}
