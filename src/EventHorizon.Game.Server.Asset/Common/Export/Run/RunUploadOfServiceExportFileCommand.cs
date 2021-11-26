namespace EventHorizon.Game.Server.Asset.Common.Export.Run;

using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Http;

public struct RunUploadOfServiceExportFileCommand
    : IRequest<CommandResult<RunUploadOfServiceExportFileResult>>
{
    public string Service { get; }
    public IFormFile File { get; }

    public RunUploadOfServiceExportFileCommand(
        string service,
        IFormFile file
    )
    {
        Service = service;
        File = file;
    }
}
