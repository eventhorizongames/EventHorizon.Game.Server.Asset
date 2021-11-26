namespace EventHorizon.Game.Server.Asset.Common.Export.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Common.Export.Query;
using EventHorizon.Game.Server.Asset.Common.Export.Run;
using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.FileManagement.Model;
using EventHorizon.Game.Server.Asset.Policies;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/[Controller]")]
[Authorize(UserIdOrAdminPolicy.PolicyName)]
public class ExportController
    : Controller
{
    private readonly ISender _sender;

    public ExportController(
        ISender sender
    )
    {
        _sender = sender;
    }

    [HttpGet("ArtifactList")]
    [ProducesResponseType(
        typeof(CommandResult<IEnumerable<ExportArtifact>>),
        StatusCodes.Status200OK
    )]
    public async Task<IActionResult> ArtifactList() => Ok(
        await _sender.Send(
            new QueryForExportArtifactList()
        )
    );

    [HttpPost("{service}/Upload")]
    [Consumes("multipart/form-data", "file-import/import")]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UploadExportFileModel), StatusCodes.Status201Created)]
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
                    ErrorCode = CommonExportErrorCodes.COMMON_EXPORT_MISSING_API_FILE_ARGUMENT,
                }
            );
        }

        var result = await _sender.Send(
            new RunUploadOfServiceExportFileCommand(
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
                    Message = "Failed to Upload Export file",
                    ErrorCode = result.ErrorCode,
                }
            );
        }

        return Created(
            result.Result.ExportPath,
            new UploadExportFileModel(
                result.Result
            )
        );
    }

    private class UploadExportFileModel
    {
        public string Service { get; }
        public string Path { get; }

        public UploadExportFileModel(
            RunUploadOfServiceExportFileResult result
        )
        {
            Service = result.Service;
            Path = result.ExportPath;
        }
    }
}
