namespace EventHorizon.Game.Server.Asset.Backup.Tasks;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.BackgroundTasks.Model;
using EventHorizon.Game.Server.Asset.Backup.ClientActions;
using EventHorizon.Game.Server.Asset.Core.Api;

using MediatR;

using Microsoft.AspNetCore.Hosting;

public class BackupAssetsTaskHandler
    : BackgroundTaskHandler<BackupAssetsTask>
{
    private readonly IWebHostEnvironment _environment;
    private readonly IPublisher _publisher;
    private readonly AssetServerSettings _settings;

    public BackupAssetsTaskHandler(
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
        BackupAssetsTask request,
        CancellationToken cancellationToken
    )
    {
        var service = "Asset".ToLowerInvariant();
        var pathToZip = Path.Combine(
            _environment.ContentRootPath,
            _settings.AssetDirectory
        );
        var destinationPath = Path.Combine(
            _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.BackupsDirectory,
            service
        );
        var backupFileName = $"backup.{DateTimeOffset.UtcNow.Ticks}.{request.ReferenceId}.zip";
        var backupServicePath = backupFileName.ToServicePath(
            _settings.BackupsDirectory,
            service
        );
        var destinationOfZip = Path.Combine(
            destinationPath,
            backupFileName
        );

        Directory.CreateDirectory(
            destinationPath
        );

        CleanupOlderBackups(
            destinationPath,
            _settings.TierSettings.MaxBackups
        );

        ZipPathIntoDestination(
            pathToZip,
            destinationOfZip
        );

        await PublishChangeEvent(
            request,
            backupServicePath,
            cancellationToken
        );

        return new();
    }

    private async Task PublishChangeEvent(
        BackupAssetsTask request,
        string backupPath,
        CancellationToken cancellationToken
    )
    {
        await _publisher.Publish(
            new ClientActionFinishedAssetBackupEvent(
                request.ReferenceId,
                backupPath
            ),
            cancellationToken
        );
    }

    private static void CleanupOlderBackups(
        string backupDestination,
        int maxBackups
    )
    {
        var filesToDelete = Directory.GetFiles(
            backupDestination
        ).OrderBy(
            a => a
        ).Reverse()
        .Skip(
            maxBackups - 1
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
