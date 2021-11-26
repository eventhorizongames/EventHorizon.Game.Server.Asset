namespace EventHorizon.Game.Server.Asset.Common.Import.Run;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Import.ClientActions;
using EventHorizon.Game.Server.Asset.Common.Import.Model;
using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RunUploadOfServiceImportFileCommandHandler
    : IRequestHandler<RunUploadOfServiceImportFileCommand, CommandResult<RunUploadOfServiceImportFileResult>>
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IPublisher _publisher;
    private readonly AssetServerSettings _settings;

    public RunUploadOfServiceImportFileCommandHandler(
        ILogger<RunUploadOfServiceImportFileCommandHandler> logger,
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

    public async Task<CommandResult<RunUploadOfServiceImportFileResult>> Handle(
        RunUploadOfServiceImportFileCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var file = request.File;

            var (fullName, fileName) = SaveFileIntoImportsDirectory(
                request.Service,
                file
            );
            var importPath = fileName.ToServicePath(
                 _settings.ImportsDirectory,
                 request.Service
             );

            CleanupOldImportedFiles(
                request.Service
            );

            await _publisher.Publish(
                new ClientActionFinishedAssetImportUploadEvent(
                    request.Service,
                    importPath
                ),
                cancellationToken
            );

            return new RunUploadOfServiceImportFileResult(
                request.Service,
                fullName,
                importPath
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to Upload Import File."
            );

            return CommonImportErrorCodes.COMMON_IMPORT_UPLOAD_ERROR;
        }
    }

    private (string FullName, string FileName) SaveFileIntoImportsDirectory(
        string service,
        IFormFile file
    )
    {
        var name = $"import.{DateTimeOffset.UtcNow.Ticks}.{Guid.NewGuid()}.{file.FileName.Trim()}";
        var fullDirectoryName = Path.Combine(
            _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.ImportsDirectory,
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

    private void CleanupOldImportedFiles(
        string service
    )
    {
        var importsDirectoryFullName = Path.Combine(
           _environment.ContentRootPath,
            _settings.DataDirectory,
            _settings.ImportsDirectory,
            service
       );
        var filesToDelete = Directory.GetFiles(
            importsDirectoryFullName
        ).OrderBy(
            fileFullName => fileFullName
        ).Reverse()
        .Skip(
            _settings.TierSettings.MaxImports
        );
        foreach (var fileFullName in filesToDelete)
        {
            File.Delete(
                fileFullName
            );
        }
    }
}
