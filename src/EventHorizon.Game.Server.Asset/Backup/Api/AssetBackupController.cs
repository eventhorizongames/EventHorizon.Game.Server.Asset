namespace EventHorizon.Game.Server.Asset.Backup.Api;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Backup.Model;
using EventHorizon.Game.Server.Asset.Backup.Trigger;
using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Policies;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("api/Asset/Backup")]
[Authorize(UserIdOrClientIdOrAdminPolicy.PolicyName)]
public class AssetBackupController
    : Controller
{
    private readonly ISender _sender;

    public AssetBackupController(
        ISender sender
    )
    {
        _sender = sender;
    }

    [HttpGet("Trigger")]
    [ProducesResponseType(
        typeof(CommandResult<BackupTriggerResult>),
        StatusCodes.Status202Accepted
    )]
    public async Task<IActionResult> Trigger() => Accepted(
        await _sender.Send(
            new TriggerBackupCommand()
        )
    );
}
