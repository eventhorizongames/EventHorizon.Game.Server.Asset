﻿namespace EventHorizon.Game.Server.Asset.Export.Tasks
{
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
        private readonly AssetServerContentDirectories _directories;

        public ExportAssetsTaskHandler(
            IWebHostEnvironment environment,
            IPublisher publisher,
            AssetServerContentDirectories directories
        )
        {
            _environment = environment;
            _publisher = publisher;
            _directories = directories;
        }

        public async Task<BackgroundTaskResult> Handle(
            ExportAssetsTask request,
            CancellationToken cancellationToken
        )
        {
            var pathToZip = Path.Combine(
                _environment.ContentRootPath,
                _directories.AssetDirectory
            );
            var destinationPath = Path.Combine(
                _environment.ContentRootPath,
                _directories.DataDirectory,
                _directories.ExportsDirectory
            );
            var exportFileName = $"export.{DateTimeOffset.UtcNow.Ticks}.{request.ReferenceId}.zip";
            var destinationOfZip = Path.Combine(
                destinationPath,
                exportFileName
            );

            CleanupOlderExports(
                destinationPath
            );

            ZipPathIntoDestination(
                pathToZip,
                destinationOfZip
            );

            await PublishChangeEvent(
                request,
                exportFileName,
                cancellationToken
            );

            return new();
        }

        private async Task PublishChangeEvent(
            ExportAssetsTask request,
            string exportFileName,
            CancellationToken cancellationToken
        )
        {
            var exportPath = $"/{_directories.ExportsDirectory}/{exportFileName}";
            await _publisher.Publish(
                new ClientActionFinishedAssetExportEvent(
                    request.ReferenceId,
                    exportPath
                ),
                cancellationToken
            );
        }

        private static void CleanupOlderExports(
            string exportDestination
        )
        {
            var maxExports = 10;
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
}