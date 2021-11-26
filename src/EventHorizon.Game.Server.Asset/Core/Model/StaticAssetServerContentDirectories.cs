namespace EventHorizon.Game.Server.Asset.Core.Model;
using EventHorizon.Game.Server.Asset.Core.Api;

public class StaticAssetServerContentDirectories
    : AssetServerSettings
{
    public string DataDirectory { get; } = "App_Data";
    public string AssetsDirectory { get; } = "Assets";
    public string AssetDirectory { get; } = "wwwroot";
    public string ExportsDirectory { get; } = "Exports";
    public string ImportsDirectory { get; } = "Imports";
    public string BackupsDirectory { get; } = "Backups";

    public ConfigurationBasedAssetServerTierSettings Tier { get; set; } = new ConfigurationBasedAssetServerTierSettings();
    AssetServerTierSettings AssetServerSettings.TierSettings => Tier;
}
