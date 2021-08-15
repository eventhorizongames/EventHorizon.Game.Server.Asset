namespace EventHorizon.Game.Server.Asset.Core.Api
{
    public interface AssetServerContentDirectories
    {
        string DataDirectory { get; }
        string AssetsDirectory { get; }
        string AssetDirectory { get; }
        string ExportsDirectory { get; }
        string ImportsDirectory { get; }
    }
}
