namespace EventHorizon.Game.Server.Asset.Core.Cleanup;
using EventHorizon.Game.Server.Asset.Core.Command;

using MediatR;

public class CleanupDirectoryCommand
    : IRequest<StandardCommandResult>
{
    public string CleanupPath { get; }

    public CleanupDirectoryCommand(
        string cleanupPath
    )
    {
        CleanupPath = cleanupPath;
    }
}
