namespace EventHorizon.Game.Server.Asset.FileManagement.Model
{
    using System.Collections.Generic;

    public class FileSystemResponse
    {
        public FileSystemDirectoryContent CWD { get; set; }

        public IEnumerable<FileSystemDirectoryContent> Files { get; set; }

        public ErrorDetails Error { get; set; }

        public FileDetails Details { get; set; }
    }
}
