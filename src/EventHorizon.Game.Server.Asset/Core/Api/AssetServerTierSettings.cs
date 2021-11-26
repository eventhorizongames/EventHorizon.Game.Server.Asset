namespace EventHorizon.Game.Server.Asset.Core.Api;

public interface AssetServerTierSettings
{
    TierLevel Level { get; }
    int MaxImports { get; }
    int MaxExports { get; }
    int MaxBackups { get; }
}
