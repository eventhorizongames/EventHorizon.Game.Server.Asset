namespace EventHorizon.Game.Server.Asset.Export.Query
{
    using System.Collections.Generic;

    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.Export.Model;

    using MediatR;

    public struct QueryForExportArtifactList
        : IRequest<CommandResult<IEnumerable<ExportArtifact>>>
    {
    }
}
