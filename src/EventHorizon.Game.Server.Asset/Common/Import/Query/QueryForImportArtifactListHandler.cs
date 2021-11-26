namespace EventHorizon.Game.Server.Asset.Common.Import.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Import.Api;
using EventHorizon.Game.Server.Asset.Common.Import.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public class QueryForImportArtifactListHandler
    : IRequestHandler<QueryForImportArtifactList, CommandResult<IEnumerable<ImportArtifact>>>
{
    private readonly ImportArtifactService _service;

    public QueryForImportArtifactListHandler(
        ImportArtifactService service
    )
    {
        _service = service;
    }

    public Task<CommandResult<IEnumerable<ImportArtifact>>> Handle(
        QueryForImportArtifactList request,
        CancellationToken cancellationToken
    ) => new CommandResult<IEnumerable<ImportArtifact>>(
        _service.ArtifactList()
    ).FromResult();
}
