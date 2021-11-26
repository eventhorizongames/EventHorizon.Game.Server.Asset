namespace EventHorizon.Game.Server.Asset.Export.Trigger;
using EventHorizon.Game.Server.Asset.Core.Command;
using EventHorizon.Game.Server.Asset.Export.Model;

using MediatR;

public struct TriggerExportCommand
    : IRequest<CommandResult<ExportTriggerResult>>
{
}
