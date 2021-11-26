namespace EventHorizon.Game.Server.Asset.Core.Cleanup;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Core.Model;

using MediatR;

using Microsoft.Extensions.Logging;

public class CleanupDirectoryCommandHandler
    : IRequestHandler<CleanupDirectoryCommand, StandardCommandResult>
{
    private readonly ILogger _logger;

    public CleanupDirectoryCommandHandler(
        ILogger<CleanupDirectoryCommandHandler> logger
    )
    {
        _logger = logger;
    }

    public Task<StandardCommandResult> Handle(
        CleanupDirectoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var cleanupPath = request.CleanupPath;
        var directoriesToDelete = Directory.GetDirectories(
            cleanupPath
        );
        foreach (var directoryToDelete in directoriesToDelete)
        {
            Directory.Delete(
                directoryToDelete,
                true
            );
        }

        var filesToDelete = Directory.GetFiles(
            cleanupPath
        );
        foreach (var fileToDelete in filesToDelete)
        {
            File.Delete(
                fileToDelete
            );
        }

        if (Directory.GetFiles(
            cleanupPath
        ).Any() || Directory.GetDirectories(
            cleanupPath
        ).Any())
        {
            _logger.LogError(
                "Failed to Cleanup Path : '{CleanupPath}'",
                cleanupPath
            );
            return new StandardCommandResult(
                CoreErrorCodes.FAILED_TO_CLEAN_EXISTING_PATH
            ).FromResult();
        }

        return new StandardCommandResult().FromResult();
    }
}
