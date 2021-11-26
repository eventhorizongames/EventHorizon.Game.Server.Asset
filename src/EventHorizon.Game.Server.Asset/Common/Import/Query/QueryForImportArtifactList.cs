namespace EventHorizon.Game.Server.Asset.Common.Import.Query;
using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Import.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public struct QueryForImportArtifactList
    : IRequest<CommandResult<IEnumerable<ImportArtifact>>>
{
}
