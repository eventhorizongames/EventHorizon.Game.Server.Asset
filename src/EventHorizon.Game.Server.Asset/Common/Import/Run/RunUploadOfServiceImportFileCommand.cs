namespace EventHorizon.Game.Server.Asset.Common.Import.Run;

using EventHorizon.Game.Server.Asset.Common.Import.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Http;

public struct RunUploadOfServiceImportFileCommand
    : IRequest<CommandResult<RunUploadOfServiceImportFileResult>>
{
    public string Service { get; }
    public IFormFile File { get; }

    public RunUploadOfServiceImportFileCommand(
        string service,
        IFormFile file
    )
    {
        Service = service;
        File = file;
    }
}
