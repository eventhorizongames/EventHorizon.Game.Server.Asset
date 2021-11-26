namespace EventHorizon.Game.Server.Asset.Common.Backup.Run;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Backup.ClientActions;
using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RunUploadOfServiceBackupFileCommandHandler
    : IRequestHandler<RunUploadOfServiceBackupFileCommand, CommandResult<RunUploadOfServiceBackupFileResult>>
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IPublisher _publisher;
    private readonly AssetServerSettings _settings;

    public RunUploadOfServiceBackupFileCommandHandler(
        ILogger<RunUploadOfServiceBackupFileCommandHandler> logger,
        IWebHostEnvironment environment,
        IPublisher publisher,
        AssetServerSettings settings
    )
    {
        _logger = logger;
        _environment = environment;
        _publisher = publisher;
        _settings = settings;
    }

    public async Task<CommandResult<RunUploadOfServiceBackupFileResult>> Handle(
        RunUploadOfServiceBackupFileCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var file = request.File;

            var (fullName, fileName) = SaveFileIntoBackupsDirectory(
                request.Service,
                file
            );
            var backupPath = fileName.ToServicePath(
                _settings.BackupsDirectory,
                request.Service
            );
            CleanupOldBackupedFiles(
                request.Service
            );

            await _publisher.Publish(
                new ClientActionFinishedServiceBackupUploadEvent(
                    request.Service,
                    backupPath
                ),
                cancellationToken
            );

            return new RunUploadOfServiceBackupFileResult(
                request.Service,
                fullName,
                backupPath
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to Upload Backup File."
            );

            return CommonBackupErrorCodes.COMMON_BACKUP_UPLOAD_ERROR;
        }
    }

    private (string FullName, string FileName) SaveFileIntoBackupsDirectory(
        string service,
        IFormFile file
    )
    {
        var name = $"backup.{DateTimeOffset.UtcNow.Ticks}.{Guid.NewGuid()}.{file.FileName.Trim()}";
        var fullDirectoryName = Path.Combine(
            _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.BackupsDirectory,
            service.ToLowerInvariant()
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

        return (fullName, name);
    }

    private void CleanupOldBackupedFiles(
        string service
    )
    {
        var backupsDirectoryFullName = Path.Combine(
           _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.BackupsDirectory,
            service
       );
        var filesToDelete = Directory.GetFiles(
            backupsDirectoryFullName
        ).OrderBy(
            fileFullName => fileFullName
        ).Reverse()
        .Skip(
            _settings.TierSettings.MaxBackups
        );
        foreach (var fileFullName in filesToDelete)
        {
            File.Delete(
                fileFullName
            );
        }
    }
}
