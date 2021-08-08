namespace EventHorizon.Game.Server.Asset.Export.Trigger
{
    using System.Threading;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Model;

    using MediatR;

    public class TriggerExportCommandHandler
        : IRequestHandler<TriggerExportCommand, CommandResult<ExportTriggerResult>>
    {
        private readonly ExportTriggerService _service;

        public TriggerExportCommandHandler(
            ExportTriggerService service
        )
        {
            _service = service;
        }

        public async Task<CommandResult<ExportTriggerResult>> Handle(
            TriggerExportCommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await _service.Trigger(
                cancellationToken
            );

            return new(
                result
            );
        }
    }
}
