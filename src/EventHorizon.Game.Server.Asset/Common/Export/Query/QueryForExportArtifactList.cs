namespace EventHorizon.Game.Server.Asset.Common.Export.Query;
using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Export.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public struct QueryForExportArtifactList
    : IRequest<CommandResult<IEnumerable<ExportArtifact>>>
{
}
