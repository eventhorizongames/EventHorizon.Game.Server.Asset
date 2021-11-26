namespace EventHorizon.Game.Server.Asset.Common.Export.Model;

public class RunUploadOfServiceExportFileResult
{
    public string Service { get; }
    public string ExportFileFullName { get; }
    public string ExportPath { get; }

    public RunUploadOfServiceExportFileResult(
        string service,
        string exportFileFullName,
        string exportPath
    )
    {
        Service = service;
        ExportFileFullName = exportFileFullName;
        ExportPath = exportPath;
    }
}
