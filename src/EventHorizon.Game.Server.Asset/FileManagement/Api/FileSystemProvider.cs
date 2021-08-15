namespace EventHorizon.Game.Server.Asset.FileManagement.Api
{
    using System.Collections.Generic;

    using EventHorizon.Game.Server.Asset.FileManagement.Model;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public interface FileSystemProvider
    {
        FileSystemResponse GetFiles(
            string path,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Create(
            string path,
            string name,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Details(
            string path,
            string[] names,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Delete(
            string path,
            string[] names,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Rename(
            string path,
            string name,
            string newName,
            bool replace = false,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Copy(
            string path,
            string targetPath,
            string[] names,
            string[] renameFiles,
            FileSystemDirectoryContent targetData,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Move(
            string path,
            string targetPath,
            string[] names,
            string[] renameFiles,
            FileSystemDirectoryContent targetData,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Search(
            string path,
            string searchString,
            bool caseSensitive,
            params FileSystemDirectoryContent[] data
        );

        FileStreamResult Download(
            string path,
            string[] names,
            params FileSystemDirectoryContent[] data
        );

        FileSystemResponse Upload(
            string path,
            IList<IFormFile> uploadFiles,
            string action,
            params FileSystemDirectoryContent[] data
        );
    }
}
