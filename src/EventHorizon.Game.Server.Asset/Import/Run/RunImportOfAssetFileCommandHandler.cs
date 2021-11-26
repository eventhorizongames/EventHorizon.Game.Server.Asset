namespace EventHorizon.Game.Server.Asset.Import.Run;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Backup.Tasks;
using EventHorizon.Game.Server.Asset.Common.Import.Run;
using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Core.Cleanup;
using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Import.Model;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

public class RunImportOfAssetFileCommandHandler
    : IRequestHandler<RunImportOfAssetFileCommand, StandardCommandResult>
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ISender _sender;
    private readonly AssetServerSettings _settings;

    public RunImportOfAssetFileCommandHandler(
        ILogger<RunImportOfAssetFileCommandHandler> logger,
        IWebHostEnvironment environment,
        ISender sender,
        AssetServerSettings settings
    )
    {
        _logger = logger;
        _environment = environment;
        _sender = sender;
        _settings = settings;
    }

    public async Task<StandardCommandResult> Handle(
        RunImportOfAssetFileCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var file = request.File;

            var uploadResult = await _sender.Send(
                new RunUploadOfServiceImportFileCommand(
                    "Asset",
                    file
                ),
                cancellationToken
            );

            if (!uploadResult)
            {
                return uploadResult.ErrorCode;
            }
            var fullName = uploadResult.Result.FileFullName;

            var result = await _sender.Send(
                new BackupAssetsTask(),
                cancellationToken
            );
            if (!result.Success)
            {
                return new(
                    AssetImportErrorCodes.FAILED_TO_CREATE_BACKUP
                );
            }

            var pathToUnzip = Path.Combine(
                _environment.ContentRootPath,
                _settings.AssetDirectory
            );
            var cleanupResult = await _sender.Send(
                new CleanupDirectoryCommand(
                    pathToUnzip
                ),
                cancellationToken
            );
            if (!cleanupResult)
            {
                return new(
                    cleanupResult.ErrorCode
                );
            }

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
}
