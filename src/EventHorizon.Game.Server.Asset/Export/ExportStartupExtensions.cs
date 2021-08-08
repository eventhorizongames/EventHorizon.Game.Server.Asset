namespace EventHorizon.Game.Server.Asset.Export
{
    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Services;

    using Microsoft.Extensions.DependencyInjection;

    public static class ExportStartupExtensions
    {
        public static IServiceCollection AddExportServices(
            this IServiceCollection services
        ) => services
            .AddSingleton<ExportStatusService, StandardExportStatusService>()
            .AddSingleton<ExportTriggerService, StandardExportTriggerService>()
        ;
    }
}
