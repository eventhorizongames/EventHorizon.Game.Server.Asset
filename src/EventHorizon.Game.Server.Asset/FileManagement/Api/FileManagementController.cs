namespace EventHorizon.Game.Server.Asset.FileManagement.Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.FileManagement.Model;
    using EventHorizon.Game.Server.Asset.Policies;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// https://github.com/SyncfusionExamples/ej2-aspcore-file-provider/blob/master/Models/PhysicalFileProvider.cs
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(UserIdOrAdminPolicy.PolicyName)]
    public class FileManagementController
        : Controller
    {
        private readonly FileSystemProvider _fileSystemProvider;
        private readonly string _basePath;

        public FileManagementController(
            IWebHostEnvironment hostingEnvironment,
            AssetServerContentDirectories directories,
            FileSystemProvider fileSystemProvider
        )
        {
            _basePath = hostingEnvironment.ContentRootPath;
            _fileSystemProvider = fileSystemProvider;

            _fileSystemProvider.RootFolder(Path.Combine(
                _basePath,
                directories.AssetDirectory
            ));
        }

        [HttpGet("Search")]
        [ProducesResponseType(typeof(FileSystemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status417ExpectationFailed)]
        public IActionResult Search(
            string? path = "/",
            string? searchString = "*",
            bool? caseSensitive = true
        )
        {
            var result = _fileSystemProvider.Search(
                path ?? "/",
                searchString ?? "*",
                caseSensitive ?? true
            );
            if (result.Error is not null)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(
                result
            );
        }

        [HttpGet("GetFiles")]
        [ProducesResponseType(typeof(FileSystemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status417ExpectationFailed)]
        public IActionResult GetFiles(
            string? path = "/"
        )
        {
            var result = _fileSystemProvider.GetFiles(
                path ?? "/"
            );
            if (result.Error is not null)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(
                result
            );
        }

        [HttpPost("CreateDirectory")]
        [ProducesResponseType(typeof(FileSystemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status417ExpectationFailed)]
        public IActionResult CreateDirectory(
            string? path,
            string? name
        )
        {
            if (path is null)
            {
                return BadRequest(
                    new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Path is Required."
                    }
                );
            }
            else if (name is null)
            {
                return BadRequest(
                    new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "Name is Required."
                    }
                );
            }

            var result = _fileSystemProvider.Create(
                path,
                name
            );
            if (result.Error is not null)
            {
                return StatusCode(result.Error.Code, result.Error);
            }
            var directoryPath = $"{path}/{name}";

            return Created(
                $"/api/FileManagement/GetFiles?path={HttpUtility.UrlEncode(directoryPath)}",
                result
            );
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(FileSystemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status417ExpectationFailed)]
        public IActionResult Delete(
            string? path = "/",
            string[]? names = null
        )
        {
            var result = _fileSystemProvider.Delete(
                path ?? "/",
                names ?? Array.Empty<string>()
            );

            if (result.Error is not null)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(
                result
            );
        }

        [HttpPost("Upload")]
        [Consumes("multipart/form-data", "file-upload/upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status417ExpectationFailed)]
        public IActionResult UploadFile(
            [FromForm(Name = "file")] IFormFile? file,
            [FromForm(Name = "file-path")] string? filePath,
            [FromForm(Name = "action")] string? action
        )
        {
            if (filePath is null)
            {
                return BadRequest(
                    new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "File Path is Required."
                    }
                );
            }
            else if (file is null)
            {
                return BadRequest(
                    new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "No File Found to Upload"
                    }
                );
            }

            var result = _fileSystemProvider.Upload(
                filePath,
                new List<IFormFile> { file },
                action ?? "save"
            );

            if (result.Error != null)
            {

                return StatusCode(
                    result.Error.Code,
                    result.Error
                );
            }

            return Ok();
        }
    }
}
