namespace EventHorizon.Game.Server.Asset.Import.Run;

using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Http;

public struct RunImportOfAssetFileCommand
    : IRequest<CommandResult<RunImportOfAssetFileResult>>
{
    public IFormFile File { get; }

    public RunImportOfAssetFileCommand(IFormFile file)
    {
        File = file;
    }
}

public record RunImportOfAssetFileResult(
    string Service,
    string ImportPath
);
