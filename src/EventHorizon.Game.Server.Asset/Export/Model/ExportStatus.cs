namespace EventHorizon.Game.Server.Asset.Export.Model
{
    using System;

    public class ExportStatus
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
