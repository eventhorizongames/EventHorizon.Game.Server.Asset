namespace EventHorizon.Game.Server.Asset.Export.Tasks;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.BackgroundTasks.Model;
using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Export.ClientActions;

using MediatR;

using Microsoft.AspNetCore.Hosting;

public class ExportAssetsTaskHandler
    : BackgroundTaskHandler<ExportAssetsTask>
{
    private readonly IWebHostEnvironment _environment;
    private readonly IPublisher _publisher;
    private readonly AssetServerSettings _settings;

    public ExportAssetsTaskHandler(
        IWebHostEnvironment environment,
        IPublisher publisher,
        AssetServerSettings settings
    )
    {
        _environment = environment;
        _publisher = publisher;
        _settings = settings;
    }

    public async Task<BackgroundTaskResult> Handle(
        ExportAssetsTask request,
        CancellationToken cancellationToken
    )
    {
        var pathToZip = Path.Combine(
            _environment.ContentRootPath,
            _settings.AssetDirectory
        );
        var destinationPath = Path.Combine(
            _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.ExportsDirectory,
            "Asset".ToLowerInvariant()
        );
        var exportFileName = $"export.{DateTimeOffset.UtcNow.Ticks}.{request.ReferenceId}.zip";
        var exportServicePath = exportFileName.ToServicePath(
            _settings.ExportsDirectory,
            "asset"
        );
        var destinationOfZip = Path.Combine(
            destinationPath,
            exportFileName
        );

        Directory.CreateDirectory(
            destinationPath
        );
        CleanupOlderExports(
            destinationPath,
            _settings.TierSettings.MaxExports
        );

        ZipPathIntoDestination(
            pathToZip,
            destinationOfZip
        );

        await PublishChangeEvent(
            request,
            exportServicePath,
            cancellationToken
        );

        return new();
    }

    private async Task PublishChangeEvent(
        ExportAssetsTask request,
        string exportPath,
        CancellationToken cancellationToken
    )
    {
        await _publisher.Publish(
            new ClientActionFinishedAssetExportEvent(
                request.ReferenceId,
                exportPath
            ),
            cancellationToken
        );
    }

    private static void CleanupOlderExports(
        string exportDestination,
        int maxExports
    )
    {
        var filesToDelete = Directory.GetFiles(
            exportDestination
        ).OrderBy(
            a => a
        ).Reverse()
        .Skip(
            maxExports - 1
        );
        foreach (var fileFullName in filesToDelete)
        {
            File.Delete(
                fileFullName
            );
        }
    }

    private static void ZipPathIntoDestination(
        string pathToZip,
        string destinationOfZip
    )
    {
        ZipFile.CreateFromDirectory(
            pathToZip,
            destinationOfZip
        );
    }
}
