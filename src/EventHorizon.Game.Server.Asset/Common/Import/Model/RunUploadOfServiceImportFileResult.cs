namespace EventHorizon.Game.Server.Asset.Common.Import.Model;

public class RunUploadOfServiceImportFileResult
{
    public string Service { get; }
    public string FileFullName { get; }
    public string ImportPath { get; }

    public RunUploadOfServiceImportFileResult(
        string service,
        string fileFullName,
        string importPath
    )
    {
        Service = service;
        FileFullName = fileFullName;
        ImportPath = importPath;
    }
}
