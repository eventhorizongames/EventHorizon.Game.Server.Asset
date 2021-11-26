namespace EventHorizon.Game.Server.Asset.Common.Export.Api;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Export.Model;
using EventHorizon.Game.Server.Asset.Export.Trigger;
using EventHorizon.Game.Server.Asset.Policies;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/Asset/Export")]
[Authorize(UserIdOrAdminPolicy.PolicyName)]
public class AssetExportController
    : Controller
{
    private readonly ISender _sender;

    public AssetExportController(
        ISender sender
    )
    {
        _sender = sender;
    }

    [HttpGet("Trigger")]
    [ProducesResponseType(
        typeof(CommandResult<ExportTriggerResult>),
        StatusCodes.Status202Accepted
    )]
    public async Task<IActionResult> Trigger() => Accepted(
        await _sender.Send(
            new TriggerExportCommand()
        )
    );
}
