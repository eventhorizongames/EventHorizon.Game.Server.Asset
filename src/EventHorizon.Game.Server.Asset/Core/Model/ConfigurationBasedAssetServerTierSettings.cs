namespace EventHorizon.Game.Server.Asset.Core.Model;
using EventHorizon.Game.Server.Asset.Core.Api;

public class ConfigurationBasedAssetServerTierSettings
    : AssetServerTierSettings
{
    public TierLevel Level { get; set; } = TierLevel.Demo;
    public int MaxImports { get; set; } = 3;
    public int MaxExports { get; set; } = 3;
    public int MaxBackups { get; set; } = 3;
}
