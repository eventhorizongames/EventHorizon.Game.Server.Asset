namespace EventHorizon.Game.Server.Asset.FileManagement.Model
{
    public class AssetDetails
    {
        public string ParentPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string FullName => $"{ParentPath}{Name}";
    }
}
