namespace EventHorizon.Game.Server.Asset.Core.Api;

public interface AssetServerSettings
{
    string DataDirectory { get; }
    string AssetsDirectory { get; }
    string AssetDirectory { get; }
    string ExportsDirectory { get; }
    string ImportsDirectory { get; }
    string BackupsDirectory { get; }

    AssetServerTierSettings TierSettings { get; }
}
