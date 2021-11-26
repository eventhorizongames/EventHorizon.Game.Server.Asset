namespace EventHorizon.Game.Server.Asset.FileManagement.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.Core.Command;
    using EventHorizon.Game.Server.Asset.FileManagement.Api;
    using EventHorizon.Game.Server.Asset.FileManagement.Model;

    using MediatR;

    public class QueryForFileManagementAssetsHandler
        : IRequestHandler<QueryForFileManagementAssets, CommandResult<FileManagementAssets>>
    {
        private readonly AssetServerSettings _settings;
        private readonly FileSystemProvider _fileSystemProvider;

        public QueryForFileManagementAssetsHandler(
            AssetServerSettings settings,
            FileSystemProvider fileSystemProvider
        )
        {
            _settings = settings;
            _fileSystemProvider = fileSystemProvider;
        }

        public Task<CommandResult<FileManagementAssets>> Handle(
            QueryForFileManagementAssets request,
            CancellationToken cancellationToken
        )
        {
            var found = _fileSystemProvider.Search(
                "/",
                "*",
                true
            );

            string ParentPath(
                FileSystemDirectoryContent content
            )
            {
                return $"/{_settings.AssetsDirectory}{content.FilterPath.Replace("\\", "/")}";
            }

            return new CommandResult<FileManagementAssets>(
                new FileManagementAssets
                {
                    FileList = found.Files?.Where(
                        content => content.IsFile
                    ).Select(
                        file => new AssetDetails
                        {
                            ParentPath = ParentPath(file),
                            Name = file.Name,
                        }
                    ) ?? new List<AssetDetails>(),

                    PathList = found.Files?.Where(
                        content => !content.IsFile
                    ).Select(
                        path => new AssetDetails
                        {
                            ParentPath = ParentPath(path),
                            Name = path.Name,
                        }
                    ) ?? new List<AssetDetails>(),
                }
            ).FromResult();
        }
    }
}
