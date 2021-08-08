namespace EventHorizon.Game.Server.Asset.Export.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.Export.Model;
    using EventHorizon.Game.Server.Asset.Export.Query;
    using EventHorizon.Game.Server.Asset.Export.Trigger;
    using EventHorizon.Game.Server.Asset.Policies;

    using MediatR;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
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

        [HttpGet("Trigger")]
        [ProducesResponseType(typeof(CommandResult<ExportTriggerResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Trigger()
        {
            return Ok(
                await _sender.Send(
                    new TriggerExportCommand()
                )
            );
        }

        [HttpGet("ArtifactList")]
        [ProducesResponseType(typeof(CommandResult<IEnumerable<ExportArtifact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ArtifactList() => Ok(
                await _sender.Send(
                    new QueryForExportArtifactList()
                )
            );
    }
}
