namespace EventHorizon.Game.Server.Asset.Common.Backup.Run;

using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

using Microsoft.AspNetCore.Http;

public struct RunUploadOfServiceBackupFileCommand
    : IRequest<CommandResult<RunUploadOfServiceBackupFileResult>>
{
    public string Service { get; }
    public IFormFile File { get; }

    public RunUploadOfServiceBackupFileCommand(
        string service,
        IFormFile file
    )
    {
        Service = service;
        File = file;
    }
}
