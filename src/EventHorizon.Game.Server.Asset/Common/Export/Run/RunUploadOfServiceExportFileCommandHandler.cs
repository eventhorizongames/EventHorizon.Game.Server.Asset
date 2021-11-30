namespace EventHorizon.Game.Server.Asset.Common.Export.Run;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Export.ClientActions;
using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RunUploadOfServiceExportFileCommandHandler
    : IRequestHandler<RunUploadOfServiceExportFileCommand, CommandResult<RunUploadOfServiceExportFileResult>>
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IPublisher _publisher;
    private readonly AssetServerSettings _settings;

    public RunUploadOfServiceExportFileCommandHandler(
        ILogger<RunUploadOfServiceExportFileCommandHandler> logger,
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

    public async Task<CommandResult<RunUploadOfServiceExportFileResult>> Handle(
        RunUploadOfServiceExportFileCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var service = request.Service.ToLowerInvariant();
            var file = request.File;

            var (fullName, fileName) = SaveFileIntoExportsDirectory(
                service,
                file
            );
            var exportPath = fileName.ToServicePath(
                _settings.ExportsDirectory,
                service
            );
            CleanupOldExportedFiles(
                service
            );

            await _publisher.Publish(
                new ClientActionFinishedServiceExportUploadEvent(
                    service,
                    exportPath
                ),
                cancellationToken
            );

            return new RunUploadOfServiceExportFileResult(
                service,
                fullName,
                exportPath
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to Upload Export File."
            );

            return CommonExportErrorCodes.COMMON_EXPORT_UPLOAD_ERROR;
        }
    }

    private (string FullName, string FileName) SaveFileIntoExportsDirectory(
        string service,
        IFormFile file
    )
    {
        var name = $"export.{DateTimeOffset.UtcNow.Ticks}.{Guid.NewGuid()}.{file.FileName.Trim()}";
        var fullDirectoryName = Path.Combine(
            _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.ExportsDirectory,
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

    private void CleanupOldExportedFiles(
        string service
    )
    {
        var exportsDirectoryFullName = Path.Combine(
           _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.ExportsDirectory,
            service
       );
        var filesToDelete = Directory.GetFiles(
            exportsDirectoryFullName
        ).OrderBy(
            fileFullName => fileFullName
        ).Reverse()
        .Skip(
            _settings.TierSettings.MaxExports
        );
        foreach (var fileFullName in filesToDelete)
        {
            File.Delete(
                fileFullName
            );
        }
    }
}
