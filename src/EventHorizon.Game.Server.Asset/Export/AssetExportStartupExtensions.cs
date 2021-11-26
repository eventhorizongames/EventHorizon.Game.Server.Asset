namespace EventHorizon.Game.Server.Asset.Export;
using EventHorizon.Game.Server.Asset.Export.Api;
using EventHorizon.Game.Server.Asset.Export.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class AssetExportStartupExtensions
{
    public static IServiceCollection AddAssetExportServices(
        this IServiceCollection services
    ) => services
        .AddSingleton<ExportTriggerService, StandardExportTriggerService>()
    ;

    public static IApplicationBuilder UseAssetExport(
        this IApplicationBuilder app
    ) => app;
}
