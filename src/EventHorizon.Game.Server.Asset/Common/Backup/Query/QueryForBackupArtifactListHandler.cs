namespace EventHorizon.Game.Server.Asset.Common.Backup.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Common.Backup.Api;
using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public class QueryForBackupArtifactListHandler
    : IRequestHandler<QueryForBackupArtifactList, CommandResult<IEnumerable<BackupArtifact>>>
{
    private readonly BackupArtifactService _service;

    public QueryForBackupArtifactListHandler(
        BackupArtifactService service
    )
    {
        _service = service;
    }

    public Task<CommandResult<IEnumerable<BackupArtifact>>> Handle(
        QueryForBackupArtifactList request,
        CancellationToken cancellationToken
    ) => new CommandResult<IEnumerable<BackupArtifact>>(
        _service.ArtifactList()
    ).FromResult();
}
