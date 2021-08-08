namespace EventHorizon.Game.Server.Asset.Export.Services
{
    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Model;

    internal class StandardExportStatusService
        : ExportStatusService
    {
        public ExportStatus Status()
        {
            return new ExportStatus();
        }
    }
}
