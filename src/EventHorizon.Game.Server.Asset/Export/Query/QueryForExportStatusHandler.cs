namespace EventHorizon.Game.Server.Asset.Export.Query
{
    using System.Threading;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Model;

    using MediatR;

    public class QueryForExportStatusHandler
        : IRequestHandler<QueryForExportStatus, ExportStatus>
    {
        private readonly ExportStatusService _exportService;

        public QueryForExportStatusHandler(
            ExportStatusService exportService
        )
        {
            _exportService = exportService;
        }

        public Task<ExportStatus> Handle(
            QueryForExportStatus request,
            CancellationToken cancellationToken
        )
        {
            return _exportService
                .Status()
                .FromResult();
        }
    }
}
