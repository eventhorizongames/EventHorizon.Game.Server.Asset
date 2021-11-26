namespace EventHorizon.Game.Server.Asset.Common.Backup.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Common.Backup.Query;
using EventHorizon.Game.Server.Asset.Common.Backup.Run;
using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.FileManagement.Model;
using EventHorizon.Game.Server.Asset.Policies;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[Controller]")]
[Authorize(UserIdOrAdminPolicy.PolicyName)]
public class BackupController
    : Controller
{
    private readonly ISender _sender;

    public BackupController(
        ISender sender
    )
    {
        _sender = sender;
    }

    [HttpGet("ArtifactList")]
    [ProducesResponseType(
        typeof(CommandResult<IEnumerable<BackupArtifact>>),
        StatusCodes.Status200OK
    )]
    public async Task<IActionResult> ArtifactList() => Ok(
        await _sender.Send(
            new QueryForBackupArtifactList()
        )
    );

    [HttpPost("{service}/Upload")]
    [Consumes("multipart/form-data", "file-import/import")]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UploadBackupFileModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> UploadFileAsync(
        string service,
        IFormFile? file
    )
    {
        if (file is null)
        {
            return BadRequest(
                new ErrorDetails
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "No File Found to Upload",
                    ErrorCode = CommonBackupErrorCodes.COMMON_BACKUP_MISSING_API_FILE_ARGUMENT,
                }
            );
        }

        var result = await _sender.Send(
            new RunUploadOfServiceBackupFileCommand(
                service,
                file
            )
        );

        if (!result.Success)
        {
            return BadRequest(
                new ErrorDetails
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Failed to Upload Backup file",
                    ErrorCode = result.ErrorCode,
                }
            );
        }

        return Created(
            result.Result.BackupPath,
            new UploadBackupFileModel(
                result.Result
            )
        );
    }

    private class UploadBackupFileModel
    {
        public string Service { get; }
        public string Path { get; }

        public UploadBackupFileModel(
            RunUploadOfServiceBackupFileResult result
        )
        {
            Service = result.Service;
            Path = result.BackupPath;
        }
    }
}
