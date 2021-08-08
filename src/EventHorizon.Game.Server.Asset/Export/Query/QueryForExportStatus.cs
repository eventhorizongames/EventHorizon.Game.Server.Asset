namespace EventHorizon.Game.Server.Asset.Export.Query
{
    using EventHorizon.Game.Server.Asset.Export.Model;

    using MediatR;

    public struct QueryForExportStatus
        : IRequest<ExportStatus>
    {
    }
}
