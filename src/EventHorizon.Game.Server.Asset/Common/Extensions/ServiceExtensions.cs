namespace EventHorizon.Game.Server.Asset;

internal static class ServiceExtensions
{
    internal static string ToDirectoryPath(
        this string fileName,
        string rootDirectory
    ) => $"/{rootDirectory}/{fileName}";

    internal static string ToServicePath(
        this string fileName,
        string rootDirectory,
        string service
    ) => $"/{rootDirectory}/{service}/{fileName}";
}
