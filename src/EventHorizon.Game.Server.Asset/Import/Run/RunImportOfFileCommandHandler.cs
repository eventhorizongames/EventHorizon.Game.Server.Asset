namespace EventHorizon.Game.Server.Asset.Import.Run
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.Export.Tasks;
    using EventHorizon.Game.Server.Asset.Import.Model;

    using MediatR;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class RunImportOfFileCommandHandler
        : IRequestHandler<RunImportOfFileCommand, StandardCommandResult>
    {
        private const int MAX_IMPORTS = 3;

        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ISender _sender;
        private readonly AssetServerContentDirectories _directories;

        public RunImportOfFileCommandHandler(
            ILogger<RunImportOfFileCommandHandler> logger,
            IWebHostEnvironment environment,
            ISender sender,
            AssetServerContentDirectories directories
        )
        {
            _logger = logger;
            _environment = environment;
            _sender = sender;
            _directories = directories;
        }

        public async Task<StandardCommandResult> Handle(
            RunImportOfFileCommand request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var file = request.File;

                var fullName = SaveFileIntoImportsDirectory(
                    file
                );

                CleanupOldImportedFiles();

                // Create Backup of current Assets
                var result = await _sender.Send(
                    new ExportAssetsTask(),
                    cancellationToken
                );
                if (!result.Success)
                {
                    return new(
                        AssetImportErrorCodes.FAILED_TO_CREATE_BACKUP
                    );
                }

                var cleanupResult = CleanupExistingAssets();
                if (!cleanupResult)
                {
                    return new(
                        cleanupResult.ErrorCode
                    );
                }
                var pathToUnzip = cleanupResult.Result;

                ZipFile.ExtractToDirectory(
                    fullName,
                    pathToUnzip
                );

                return new();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to Import Assets."
                );

                return new(
                    AssetImportErrorCodes.GENERAL_IMPORT_ASSETS_ERROR
                );
            }
        }

        private string SaveFileIntoImportsDirectory(
            IFormFile file
        )
        {
            var name = $"import.{DateTimeOffset.UtcNow.Ticks}.{file.FileName.Trim()}";
            var fullDirectoryName = Path.Combine(
                _environment.ContentRootPath,
                _directories.DataDirectory,
                _directories.ImportsDirectory
            );
            var fullName = Path.Combine(
                fullDirectoryName,
                name
            );
            Directory.CreateDirectory(
                fullDirectoryName
            );
            using (var fs = File.Create(fullName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            return fullName;
        }

        private void CleanupOldImportedFiles()
        {
            var importsDirectoryFullName = Path.Combine(
               _environment.ContentRootPath,
                _directories.DataDirectory,
                _directories.ImportsDirectory
           );
            var filesToDelete = Directory.GetFiles(
                importsDirectoryFullName
            ).OrderBy(
                fileFullName => fileFullName
            ).Reverse()
            .Skip(
                MAX_IMPORTS
            );
            foreach (var fileFullName in filesToDelete)
            {
                File.Delete(
                    fileFullName
                );
            }
        }

        private CommandResult<string> CleanupExistingAssets()
        {
            var pathToUnzip = Path.Combine(
                _environment.ContentRootPath,
                _directories.AssetDirectory
            );
            var directoriesToDelete = Directory.GetDirectories(
                pathToUnzip
            );
            foreach (var directoryToDelete in directoriesToDelete)
            {
                Directory.Delete(
                    directoryToDelete,
                    true
                );
            }

            var filesToDelete = Directory.GetFiles(
                pathToUnzip
            );
            foreach (var fileToDelete in filesToDelete)
            {
                File.Delete(
                    fileToDelete
                );
            }

            if (Directory.GetFiles(
                pathToUnzip
            ).Any() || Directory.GetDirectories(
                pathToUnzip
            ).Any())
            {
                return new(
                    AssetImportErrorCodes.FAILED_TO_CLEAN_EXISTING_ASSETS
                );
            }

            return new(
                success: true,
                pathToUnzip
            );
        }
    }
}
