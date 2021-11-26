namespace EventHorizon.Game.Server.Asset.Common.Model;

using System;
using System.Linq;

public class ExportArtifactBase
{
    public string Service { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ReferenceId => ReferenceIdFromPath(
        Path
    );
    public string Created => DateTimeFromPath(
        Path
    );

    static string DateTimeFromPath(
        string path
    )
    {
        var ticksAsString = System.IO.Path.GetFileName(
            path
        ).Split(
            '.'
        ).Skip(
            1
        ).FirstOrDefault() ?? "0";
        var found = long.TryParse(
            ticksAsString,
            out var ticks
        );
        if (!found)
        {
            return DateTimeOffset.MinValue.ToString("G");
        }

        return new DateTimeOffset(
            ticks,
            DateTimeOffset.Now.Offset
        ).ToString();
    }

    static string ReferenceIdFromPath(
        string path
    )
    {
        return System.IO.Path.GetFileName(
            path
        ).Split(
            '.'
        ).Skip(
            2
        ).FirstOrDefault() ?? "reference-id";
    }
}
