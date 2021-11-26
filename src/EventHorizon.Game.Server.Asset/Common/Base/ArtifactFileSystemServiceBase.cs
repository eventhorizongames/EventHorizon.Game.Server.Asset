namespace EventHorizon.Game.Server.Asset.Common.Base;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal abstract class ArtifactFileSystemServiceBase
{
    private readonly string _artifactPath;
    private readonly string _rootDirectory;

    public ArtifactFileSystemServiceBase(
        string artifactPath,
        string rootDirectory
    )
    {
        _artifactPath = artifactPath;
        _rootDirectory = rootDirectory; 
    }

    protected IEnumerable<ServiceArtifact> ReadArtifactPath()
    {
        string GetServiceName(
            string fileFullName
        )
        {
            var serviceName = Path.GetDirectoryName(
                fileFullName
            )?.Split(
                Path.DirectorySeparatorChar
            ).LastOrDefault()
            ?.ToLowerInvariant();
            var artifcatPath = _artifactPath.Split(
                Path.DirectorySeparatorChar
            ).LastOrDefault()
            ?.ToLowerInvariant() ?? string.Empty;

            if (serviceName == artifcatPath)
            {
                serviceName = string.Empty;
            }

            return serviceName.IsNotBlank()
                ? serviceName
                : string.Empty;
        }

        string GetPath(
            string service,
            string fileFullName
        )
        {
            var fileName = Path.GetFileName(fileFullName);
            if (service.IsBlank())
            {
                return fileName.ToDirectoryPath(
                    _rootDirectory
                );
            }

            return fileName.ToServicePath(
                _rootDirectory,
                service
            );
        }

        return Directory.GetFiles(
            _artifactPath,
            "*",
            SearchOption.AllDirectories
        ).Select(
            fullName => new 
            {
                Service = GetServiceName(
                    fullName
                ),
                FileFullName = fullName,
            }
        ).Select(
            fileDetails => new ServiceArtifact(
                fileDetails.Service,
                GetPath(
                    fileDetails.Service,
                    fileDetails.FileFullName
                )
            )
        );
    }

    internal record ServiceArtifact(
        string Service,
        string Path
    );
}
