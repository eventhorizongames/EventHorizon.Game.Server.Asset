namespace EventHorizon.Game.Server.Asset.Export.Api
{
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.FileManagement.Model;
    using EventHorizon.Game.Server.Asset.Import.Model;
    using EventHorizon.Game.Server.Asset.Import.Run;
    using EventHorizon.Game.Server.Asset.Policies;

    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [Authorize(UserIdOrAdminPolicy.PolicyName)]
    public class ImportController
        : Controller
    {
        private readonly ISender _sender;

        public ImportController(
            ISender sender
        )
        {
            _sender = sender;
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data", "file-import/import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFileAsync(
            IFormFile? file
        )
        {
            if (file is null)
            {
                return BadRequest(
                    new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "No File Found to Import",
                        ErrorCode = AssetImportErrorCodes.MISSING_API_FILE_ARGUMENT,
                    }
                );
            }

            var result = await _sender.Send(
                new RunImportOfFileCommand(
                    file
                )
            );

            if (!result.Success)
            {
                return BadRequest(
                    new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Failed to Create Backup",
                        ErrorCode = result.ErrorCode,
                    }
                );
            }

            return Ok();
        }
    }
}
