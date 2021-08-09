namespace EventHorizon.Game.Server.Asset.FileManagement.Model
{
    using System.Collections.Generic;

    public class ErrorDetails
    {
        public int Code { get; set; }

        public string? Message { get; set; }

        public string? ErrorCode { get; set; }

        public IEnumerable<string>? FileExists { get; set; }
    }
}
