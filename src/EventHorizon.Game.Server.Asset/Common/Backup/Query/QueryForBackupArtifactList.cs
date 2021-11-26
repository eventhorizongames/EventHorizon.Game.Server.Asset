namespace EventHorizon.Game.Server.Asset.Common.Backup.Query;
using System.Collections.Generic;

using EventHorizon.Game.Server.Asset.Common.Backup.Model;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public struct QueryForBackupArtifactList
    : IRequest<CommandResult<IEnumerable<BackupArtifact>>>
{
}
