namespace EventHorizon.Game.Server.Asset.Backup;

using EventHorizon.Game.Server.Asset.Backup.Api;
using EventHorizon.Game.Server.Asset.Backup.Services;

using Microsoft.Extensions.DependencyInjection;

public static class AssetBackupStartupExtensions
{
    public static IServiceCollection AddAssetBackupServices(
        this IServiceCollection services
    ) => services
        .AddSingleton<BackupTriggerService, StandardBackupTriggerService>()
    ;
}
