namespace EventHorizon.Game.Server.Asset.FileManagement.Model
{
    using System.Collections.Generic;

    public class AccessDetails
    {
        public string Role { get; set; } = string.Empty;
        public IEnumerable<AccessRule>? AccessRules { get; set; }
    }
}
