namespace EventHorizon.Game.Server.Asset.Export.Query
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Model;

    using MediatR;

    public class QueryForExportArtifactListHandler
        : IRequestHandler<QueryForExportArtifactList, CommandResult<IEnumerable<ExportArtifact>>>
    {
        private readonly ExportArtifactService _service;

        public QueryForExportArtifactListHandler(
            ExportArtifactService service
        )
        {
            _service = service;
        }

        public Task<CommandResult<IEnumerable<ExportArtifact>>> Handle(
            QueryForExportArtifactList request,
            CancellationToken cancellationToken
        )
        {
            return new CommandResult<IEnumerable<ExportArtifact>>(
                _service.ArtifactList()
            ).FromResult();
        }
    }
}
