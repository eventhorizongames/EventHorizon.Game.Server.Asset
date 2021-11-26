namespace EventHorizon.Game.Server.Asset.Export.Api;
using System.Threading;
using System.Threading.Tasks;

using EventHorizon.Game.Server.Asset.Export.Model;

public interface ExportTriggerService
{
    Task<ExportTriggerResult> Trigger(
        CancellationToken cancellationToken
    );
}
