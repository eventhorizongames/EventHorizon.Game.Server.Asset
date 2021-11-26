namespace EventHorizon.Game.Server.Asset.FileManagement.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.FileManagement.Api;
    using EventHorizon.Game.Server.Asset.FileManagement.Model;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class PhysicalFileSystemProvider
        : FileSystemProvider
    {
        private readonly ILogger<PhysicalFileSystemProvider> _logger;

        private readonly AccessDetails _accessDetails = new();
        private readonly string _rootName = string.Empty;

        protected string contentRootPath = string.Empty;
        protected string[] allowedExtension = new string[] { "*" };
        protected string hostPath = string.Empty;
        protected string hostName = string.Empty;

        public PhysicalFileSystemProvider(
            ILogger<PhysicalFileSystemProvider> logger,
            IWebHostEnvironment hostingEnvironment,
            AssetServerSettings settings
        )
        {
            _logger = logger;
            RootFolder(Path.Combine(
                 hostingEnvironment.ContentRootPath,
                 settings.AssetDirectory
             ));
        }

        private void RootFolder(
            string name
        )
        {
            contentRootPath = name;
            hostName = new Uri(contentRootPath).Host;
            if (!string.IsNullOrEmpty(hostName))
            {
                hostPath = Path.Combine(
                    hostName,
                    contentRootPath.Substring(
                        contentRootPath.ToLower().IndexOf(hostName) + hostName.Length + 1
                    )
                );
            }
        }

        public FileSystemResponse Copy(
            string path,
            string targetPath,
            string[] names,
            string[] renameFiles,
            FileSystemDirectoryContent targetData,
            params FileSystemDirectoryContent[] data
        )
        {
            throw new NotImplementedException("Copy");
        }

        public FileSystemResponse Create(string path, string name, params FileSystemDirectoryContent[] data)
        {
            var accessMessage = string.Empty;
            var createResponse = new FileSystemResponse();
            try
            {
                var permission = GetPathPermission(path);
                if (permission != null
                    && (!permission.Read || !permission.WriteContents)
                )
                {
                    accessMessage = permission.Message;
                    throw new UnauthorizedAccessException(
                        string.Format(
                            "'{0}' is not accessible. You need permission to perform the write action.",
                            GetFileNameFromPath(_rootName + path)
                        )
                    );
                }

                string newDirectoryPath = Path.Combine(contentRootPath + path, name);

                bool directoryExist = Directory.Exists(newDirectoryPath);

                if (directoryExist)
                {
                    var exist = new DirectoryInfo(
                        newDirectoryPath
                    );
                    var errorDetails = new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = string.Format(
                            "A file or folder with the name {0} already exists.",
                            exist.Name
                        )
                    };
                    createResponse.Error = errorDetails;

                    return createResponse;
                }

                var physicalPath = GetPath(path);
                Directory.CreateDirectory(
                    newDirectoryPath
                );

                var directory = new DirectoryInfo(newDirectoryPath);
                var createData = new FileSystemDirectoryContent
                {
                    Name = directory.Name,
                    IsFile = false,
                    Size = 0,
                    DateModified = directory.LastWriteTime,
                    DateCreated = directory.CreationTime,
                    HasChild = CheckChild(directory.FullName),
                    Type = directory.Extension,
                    Permission = GetPermission(physicalPath, directory.Name, false)
                };

                createResponse.Files = new FileSystemDirectoryContent[] { createData };

                return createResponse;
            }
            catch (Exception e)
            {
                var er = new ErrorDetails
                {
                    Message = e.Message,
                };
                er.Code = er.Message.Contains("is not accessible. You need permission")
                    ? StatusCodes.Status401Unauthorized
                    : StatusCodes.Status417ExpectationFailed;
                if (er.Code == StatusCodes.Status401Unauthorized
                    && accessMessage.IsNotBlank()
                )
                {
                    er.Message = accessMessage;
                }
                createResponse.Error = er;

                return createResponse;
            }
        }

        public FileSystemResponse Delete(
            string path,
            string[] names,
            params FileSystemDirectoryContent[] data
        )
        {
            var accessMessage = string.Empty;
            var DeleteResponse = new FileSystemResponse();
            var removedFiles = new List<FileSystemDirectoryContent>();

            try
            {
                var physicalPath = GetPath(path);
                var result = string.Empty;
                for (int i = 0; i < names.Length; i++)
                {
                    var IsFile = !IsDirectory(physicalPath, names[i]);
                    var permission = GetPermission(
                        physicalPath,
                        names[i],
                        IsFile
                    );
                    if (permission != null
                        && (!permission.Read || !permission.Write)
                    )
                    {
                        accessMessage = permission.Message;
                        throw new UnauthorizedAccessException(
                            string.Format(
                                "'{0}' is not accessible. You need permission to perform the write action.",
                                GetFileNameFromPath(_rootName + path + names[i]))
                        );
                    }
                }
                FileSystemDirectoryContent removingFile;
                for (int i = 0; i < names.Length; i++)
                {
                    var fullPath = Path.Combine(contentRootPath + path, names[i]);
                    var directory = new DirectoryInfo(fullPath);
                    if (!string.IsNullOrEmpty(names[i]))
                    {
                        FileAttributes attr = File.GetAttributes(fullPath);
                        removingFile = GetFileDetails(fullPath);
                        //detect whether its a directory or file
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            result = DeleteDirectory(fullPath);
                        }
                        else
                        {
                            try
                            {
                                File.Delete(fullPath);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to Delete path of {Path}", path);

                                if (ex.GetType().Name == "UnauthorizedAccessException")
                                {
                                    result = fullPath;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        if (result != string.Empty)
                        {
                            break;

                        }
                        removedFiles.Add(removingFile);
                    }
                    else
                    {
                        throw new ArgumentNullException(
                            nameof(names),
                            "names should not contain a null"
                        );
                    }
                }
                DeleteResponse.Files = removedFiles;
                if (result != string.Empty)
                {
                    var deniedPath = result.Substring(contentRootPath.Length);
                    var er = new ErrorDetails
                    {
                        Message = "'" + GetFileNameFromPath(deniedPath) + "' is not accessible.  you need permission to perform the write action.",
                        Code = StatusCodes.Status401Unauthorized
                    };

                    if ((er.Code == StatusCodes.Status401Unauthorized)
                        && !string.IsNullOrEmpty(accessMessage)
                    )
                    {
                        er.Message = accessMessage;
                    }
                    DeleteResponse.Error = er;
                    return DeleteResponse;
                }
                else
                {
                    return DeleteResponse;
                }
            }
            catch (Exception e)
            {
                var er = new ErrorDetails
                {
                    Message = e.Message.ToString()
                };
                er.Code = er.Message.Contains("is not accessible. You need permission")
                    ? StatusCodes.Status401Unauthorized
                    : StatusCodes.Status417ExpectationFailed;
                if ((er.Code == StatusCodes.Status401Unauthorized)
                    && !string.IsNullOrEmpty(accessMessage)
                )
                {
                    er.Message = accessMessage;
                }
                DeleteResponse.Error = er;
                return DeleteResponse;
            }
        }

        public FileSystemResponse Details(string path, string[] names, params FileSystemDirectoryContent[] data)
        {
            throw new NotImplementedException("Details");
        }

        public FileStreamResult Download(string path, string[] names, params FileSystemDirectoryContent[] data)
        {
            throw new NotImplementedException("Download");
        }

        public FileSystemResponse GetFiles(
            string path,
            params FileSystemDirectoryContent[] data
        )
        {
            var accessMessage = string.Empty;
            var readResponse = new FileSystemResponse();
            try
            {
                if (path == null)
                {
                    path = string.Empty;
                }
                var fullPath = contentRootPath + path;
                var directory = new DirectoryInfo(fullPath);
                var extensions = allowedExtension;
                var rootPath = string.IsNullOrEmpty(hostPath)
                    ? contentRootPath
                    : new DirectoryInfo(hostPath).FullName;
                var parentPath = string.IsNullOrEmpty(value: hostPath)
                    ? directory.Parent?.FullName ?? string.Empty
                    : new DirectoryInfo(hostPath + (path != "/" ? path : "")).Parent?.FullName ?? string.Empty;
                var cwd = new FileSystemDirectoryContent
                {
                    Name = string.IsNullOrEmpty(hostPath)
                    ? rootPath.EndsWith(directory.Name) ? "/" : directory.Name
                    : new DirectoryInfo(hostPath + path).Name,
                    Size = 0,
                    IsFile = false,
                    DateModified = directory.LastWriteTime,
                    DateCreated = directory.CreationTime,
                    HasChild = CheckChild(directory.FullName),
                    Type = directory.Extension,
                    FilterPath = GetRelativePath(rootPath, parentPath + Path.DirectorySeparatorChar),
                    Permission = GetPathPermission(path)
                };

                readResponse.CWD = cwd;
                if (!HasAccess(directory.FullName)
                    || (cwd.Permission != null && !cwd.Permission.Read)
                )
                {
                    readResponse.Files = null;
                    accessMessage = cwd.Permission?.Message ?? string.Empty;
                    throw new UnauthorizedAccessException(
                        string.Format(
                            "'{0}' is not accessible. You need permission to perform the read action.",
                            cwd.Name
                        )
                    );
                }
                readResponse.Files = ReadDirectories(
                    directory,
                    extensions,
                    data
                );
                readResponse.Files = readResponse.Files.Concat(
                    ReadFiles(
                        directory,
                        extensions,
                        data
                    )
                );
                return readResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get files for {Path}", path);

                var er = new ErrorDetails
                {
                    Message = ex.Message.ToString()
                };
                er.Code = er.Message.Contains("is not accessible. You need permission")
                    ? StatusCodes.Status401Unauthorized
                    : StatusCodes.Status417ExpectationFailed;
                if (er.Code == StatusCodes.Status401Unauthorized
                    && !string.IsNullOrEmpty(accessMessage)
                )
                {
                    er.Message = accessMessage;
                }
                readResponse.Error = er;
                return readResponse;
            }
        }

        public FileSystemResponse Move(string path, string targetPath, string[] names, string[] renameFiles, FileSystemDirectoryContent targetData, params FileSystemDirectoryContent[] data)
        {
            throw new NotImplementedException();
        }

        public FileSystemResponse Rename(string path, string name, string newName, bool replace = false, params FileSystemDirectoryContent[] data)
        {
            throw new NotImplementedException();
        }

        public FileSystemResponse Search(
            string path,
            string searchString,
            bool caseSensitive,
            params FileSystemDirectoryContent[] data
        )
        {
            var accessMessage = string.Empty;
            var searchResponse = new FileSystemResponse();

            try
            {
                if (path == null)
                {
                    path = string.Empty;
                };

                var searchWord = searchString;
                var searchPath = contentRootPath + path;
                var directory = new DirectoryInfo(contentRootPath + path);
                var rootPath = string.IsNullOrEmpty(hostPath)
                    ? contentRootPath
                    : new DirectoryInfo(hostPath).FullName;
                var parentPath = string.IsNullOrEmpty(hostPath)
                    ? directory.Parent?.FullName ?? string.Empty
                    : new DirectoryInfo(hostPath + (path != "/" ? path : "")).Parent?.FullName ?? string.Empty;

                var cwd = new FileSystemDirectoryContent
                {
                    Name = directory.Name,
                    Size = 0,
                    IsFile = false,
                    DateModified = directory.LastWriteTime,
                    DateCreated = directory.CreationTime,
                    HasChild = CheckChild(
                        directory.FullName
                    ),
                    Type = directory.Extension,
                    FilterPath = GetRelativePath(
                        rootPath,
                        parentPath + Path.DirectorySeparatorChar
                    ),
                    Permission = GetPathPermission(
                        path
                    ),
                };

                if (cwd.Permission != null && !cwd.Permission.Read)
                {
                    accessMessage = cwd.Permission.Message;
                    throw new UnauthorizedAccessException(
                        string.Format(
                            "'{0}' is not accessible. You need permission to perform the read action.",
                            GetFileNameFromPath(_rootName + path)
                        )
                    );
                }
                searchResponse.CWD = cwd;

                var foundedFiles = new List<FileSystemDirectoryContent>();
                var extensions = allowedExtension;
                var searchDirectory = new DirectoryInfo(searchPath);
                var files = new List<FileInfo>();
                var directories = new List<DirectoryInfo>();
                var filteredFileList = GetDirectoryFiles(
                    searchDirectory,
                    files
                ).Where(
                    item => new Regex(
                        WildcardToRegex(
                            searchString
                        ),
                        caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase
                    ).IsMatch(
                        item.Name
                    )
                );
                var filteredDirectoryList = GetDirectoryFolders(
                    searchDirectory, directories
                ).Where(
                    item => new Regex(
                        WildcardToRegex(
                            searchString
                        ),
                        caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase
                    ).IsMatch(
                        item.Name
                    )
                );

                foreach (FileInfo file in filteredFileList)
                {
                    var fileDetails = GetFileDetails(
                        Path.Combine(
                            contentRootPath,
                            file.DirectoryName ?? string.Empty,
                            file.Name
                        )
                    );
                    var hasPermission = ParentsHavePermission(fileDetails);
                    if (hasPermission)
                    {
                        foundedFiles.Add(fileDetails);
                    }
                }
                foreach (DirectoryInfo dir in filteredDirectoryList)
                {
                    var dirDetails = GetFileDetails(
                        Path.Combine(
                            contentRootPath,
                            dir.FullName
                        )
                    );
                    var hasPermission = ParentsHavePermission(dirDetails);
                    if (hasPermission)
                    {
                        foundedFiles.Add(dirDetails);
                    }
                }

                searchResponse.Files = foundedFiles;
                return searchResponse;
            }
            catch (Exception e)
            {
                var er = new ErrorDetails
                {
                    Message = e.Message.ToString()
                };
                er.Code = er.Message.Contains("is not accessible. You need permission")
                            ? StatusCodes.Status401Unauthorized
                            : StatusCodes.Status417ExpectationFailed;
                if ((er.Code == StatusCodes.Status401Unauthorized)
                    && !string.IsNullOrEmpty(accessMessage)
                )
                {
                    er.Message = accessMessage;
                }
                searchResponse.Error = er;
                return searchResponse;
            }
        }

        public FileSystemResponse Upload(
            string path,
            IList<IFormFile> uploadFiles,
            string action,
            params FileSystemDirectoryContent[] data
        )
        {
            var uploadResponse = new FileSystemResponse();
            try
            {
                var pathPermission = GetPathPermission(path);
                if (pathPermission != null
                    && (!pathPermission.Read || !pathPermission.Upload)
                )
                {
                    throw new UnauthorizedAccessException(
                        pathPermission.Message
                    );
                }

                var existFiles = new List<string>();
                foreach (var file in uploadFiles)
                {
                    var name = file.FileName.Trim().ToString();
                    var fullName = Path.Combine(
                        contentRootPath + path,
                        name
                    );
                    if (action == "save")
                    {
                        if (!File.Exists(fullName))
                        {
#pragma warning disable IDE0063 // Use simple 'using' statement
                            using (FileStream fs = File.Create(fullName))
#pragma warning restore IDE0063 // Use simple 'using' statement
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            existFiles.Add(fullName);
                        }
                    }
                    else if (action == "replace")
                    {
                        if (File.Exists(fullName))
                        {
                            File.Delete(fullName);
                        }
#pragma warning disable IDE0063 // Use simple 'using' statement
                        using (FileStream fs = File.Create(fullName))
#pragma warning restore IDE0063 // Use simple 'using' statement
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                    else if (action == "keepboth")
                    {
                        string newName = fullName;
                        int index = newName.LastIndexOf(".");
                        if (index >= 0)
                            newName = newName.Substring(0, index);
                        int fileCount = 0;
                        while (File.Exists(
                            newName + (fileCount > 0
                                ? CreateKeepFileName(name, fileCount)
                                : Path.GetExtension(name))
                        ))
                        {
                            fileCount++;
                        }
                        newName += fileCount > 0
                            ? CreateKeepFileName(name, fileCount)
                            : Path.GetExtension(name)
                        ;
#pragma warning disable IDE0063 // Use simple 'using' statement
                        using (FileStream fs = File.Create(newName))
#pragma warning restore IDE0063 // Use simple 'using' statement
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
                if (existFiles.Count != 0)
                {
                    uploadResponse.Error = new ErrorDetails
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message = "File already exists.",
                        FileExists = existFiles
                    };
                }
                return uploadResponse;
            }
            catch (UnauthorizedAccessException ex)
            {
                uploadResponse.Error = new ErrorDetails
                {
                    Code = StatusCodes.Status401Unauthorized,
                    Message = ex.Message,
                };
                return uploadResponse;
            }
            catch (Exception ex)
            {
                uploadResponse.Error = new ErrorDetails
                {
                    Code = StatusCodes.Status417ExpectationFailed,
                    Message = ex.Message,
                };
                return uploadResponse;
            }

            static string CreateKeepFileName(
                string name,
                int fileCount
            )
            {
                return "." + fileCount.ToString() + Path.GetExtension(name);
            }
        }

        protected virtual IEnumerable<FileSystemDirectoryContent> ReadDirectories(
            DirectoryInfo directory,
            string[] extensions,
            params FileSystemDirectoryContent[] data
        )
        {
            var readDirectory = new FileSystemResponse();
            try
            {
                var directories = directory.GetDirectories()
                    .Select(subDirectory => new FileSystemDirectoryContent
                    {
                        Name = subDirectory.Name,
                        Size = 0,
                        IsFile = false,
                        DateModified = subDirectory.LastWriteTime,
                        DateCreated = subDirectory.CreationTime,
                        HasChild = CheckChild(subDirectory.FullName),
                        Type = subDirectory.Extension,
                        FilterPath = GetRelativePath(contentRootPath, directory.FullName),
                        Permission = GetPermission(directory.FullName, subDirectory.Name, false)
                    }
                    );
                readDirectory.Files = directories;
                return readDirectory.Files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Read Directories at {DirectoryFullName}", directory.FullName);
                throw;
            }
        }

        protected virtual IEnumerable<FileSystemDirectoryContent> ReadFiles(
            DirectoryInfo directory,
            string[] extensions,
            params FileSystemDirectoryContent[] data
        )
        {
            try
            {
                var readFiles = new FileSystemResponse();
                var files = extensions.SelectMany(directory.GetFiles)
                    .Select(file => new FileSystemDirectoryContent
                    {
                        Name = file.Name,
                        IsFile = true,
                        Size = file.Length,
                        DateModified = file.LastWriteTime,
                        DateCreated = file.CreationTime,
                        HasChild = false,
                        Type = file.Extension,
                        FilterPath = GetRelativePath(contentRootPath, directory.FullName),
                        Permission = GetPermission(directory.FullName, file.Name, true)
                    });
                readFiles.Files = files;
                return readFiles.Files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Read Files at {DirectoryFullName}", directory.FullName);
                throw;
            }
        }

        protected string GetRelativePath(
            string rootPath,
            string fullPath
        )
        {
            if (!string.IsNullOrEmpty(
                rootPath
            ) && !string.IsNullOrEmpty(
                fullPath
            ))
            {
                DirectoryInfo rootDirectory;
                if (!string.IsNullOrEmpty(
                    hostName
                ))
                {
                    if (rootPath.Contains(
                        hostName
                    ) || rootPath.ToLower().Contains(
                        hostName
                    ) || rootPath.ToUpper().Contains(
                        hostName
                    ))
                    {
                        rootPath = rootPath.Substring(
                            rootPath.IndexOf(
                                hostName,
                                StringComparison.CurrentCultureIgnoreCase
                            ) + hostName.Length
                        );
                    }
                    if (fullPath.Contains(
                        hostName
                    ) || fullPath.ToLower().Contains(
                        hostName
                    ) || fullPath.ToUpper().Contains(
                        hostName
                    ))
                    {
                        fullPath = fullPath.Substring(
                            fullPath.IndexOf(
                                hostName,
                                StringComparison.CurrentCultureIgnoreCase
                            ) + hostName.Length
                        );
                    }

                    rootDirectory = new DirectoryInfo(
                        rootPath
                    );
                    fullPath = new DirectoryInfo(
                        fullPath
                    ).FullName;
                    rootPath = new DirectoryInfo(
                        rootPath
                    ).FullName;
                }
                else
                {
                    rootDirectory = new DirectoryInfo(rootPath);
                }

                if (rootDirectory.FullName.Substring(
                    rootDirectory.FullName.Length - 1
                ) == Path.DirectorySeparatorChar.ToString())
                {
                    if (fullPath.Contains(
                        rootDirectory.FullName
                    ))
                    {
                        return fullPath.Substring(rootPath.Length - 1);
                    }
                }
                else if (fullPath.Contains(
                    rootDirectory.FullName + Path.DirectorySeparatorChar
                ))
                {
                    return Path.DirectorySeparatorChar + fullPath.Substring(rootPath.Length + 1);
                }
            }

            return "/";
        }

        private string DeleteDirectory(string path)
        {
            try
            {
                var result = string.Empty;
                var files = Directory.GetFiles(path);
                var dirs = Directory.GetDirectories(path);
                foreach (var file in files)
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed DeleteDirectory for {Path}", path);

                        if (ex.GetType().Name == "UnauthorizedAccessException")
                        {
                            return file;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                foreach (var dir in dirs)
                {
                    result = DeleteDirectory(dir);
                    if (result != string.Empty)
                    {
                        return result;
                    }
                }
                Directory.Delete(path, true);
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed DeleteDirectory for {Path}",
                    path
                );
                return path;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed DeleteDirectory for {Path}",
                    path
                );
                throw;
            }
        }

        private string GetPath(string path)
        {
            var fullPath = contentRootPath + path;
            var directory = new DirectoryInfo(fullPath);
            return directory.FullName;
        }

        private bool CheckChild(string path)
        {
            bool hasChild;
            try
            {
                var directory = new DirectoryInfo(path);
                var dir = directory.GetDirectories();
                hasChild = dir.Length != 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to CheckChild at {Path}", path);
                if (ex.GetType().Name == "UnauthorizedAccessException")
                {
                    hasChild = false;
                }
                else
                {
                    throw;
                }
            }
            return hasChild;
        }

        private bool HasAccess(string path)
        {
            bool hasAcceess;
            try
            {
                var directory = new DirectoryInfo(path);
                var dir = directory.GetDirectories();
                hasAcceess = dir != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to HasAccess at {Path}", path);
                if (ex.GetType().Name == "UnauthorizedAccessException")
                {
                    hasAcceess = false;
                }
                else
                {
                    throw;
                }
            }
            return hasAcceess;
        }

        private FileSystemDirectoryContent GetFileDetails(
            string path
        )
        {
            try
            {
                var info = new FileInfo(path);
                var attr = File.GetAttributes(path);
                var detailPath = new FileInfo(info.FullName);
                var folderLength = 0;
                var isFile = (attr & FileAttributes.Directory) != FileAttributes.Directory;
                if (!isFile)
                {
                    folderLength = detailPath.Directory?.GetDirectories().Length ?? 0;
                }
                string filterPath = GetRelativePath(
                    contentRootPath,
                    info.DirectoryName + Path.DirectorySeparatorChar
                );
                return new FileSystemDirectoryContent
                {
                    Name = info.Name,
                    Size = isFile ? info.Length : 0,
                    IsFile = isFile,
                    DateModified = info.LastWriteTime,
                    DateCreated = info.CreationTime,
                    Type = info.Extension,
                    HasChild = !isFile && CheckChild(info.FullName),
                    FilterPath = filterPath,
                    Permission = GetPermission(
                        GetPath(filterPath),
                        info.Name,
                        isFile
                    )
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed GetFileDetails for {Path}", path);
                throw;
            }
        }

        private AccessPermission? GetPathPermission(string path)
        {
            string[] fileDetails = GetFolderDetails(path);
            return GetPermission(GetPath(fileDetails[0]), fileDetails[1], false);
        }

        private static string[] GetFolderDetails(string path)
        {
            string[] str_array = path.Split('/'), fileDetails = new string[2];
            string parentPath = "";
            for (int i = 0; i < str_array.Length - 2; i++)
            {
                parentPath += str_array[i] + "/";
            }
            fileDetails[0] = parentPath;
            fileDetails[1] = str_array[^2];
            return fileDetails;
        }

        private AccessPermission? GetPermission(
            string location,
            string name,
            bool isFile
        )
        {
            var FilePermission = new AccessPermission();
            if (isFile)
            {
                if (_accessDetails.AccessRules == null) return null;
                var nameExtension = Path.GetExtension(name).ToLower();
                var fileName = Path.GetFileNameWithoutExtension(name);
                var currentPath = GetFilePath(location + name);
                foreach (var fileRule in _accessDetails.AccessRules)
                {
                    if (!string.IsNullOrEmpty(fileRule.Path)
                        && fileRule.IsFile
                        && (fileRule.Role == null
                            || fileRule.Role == _accessDetails.Role)
                    )
                    {
                        if (fileRule.Path.IndexOf("*.*") > -1)
                        {
                            var parentPath = fileRule.Path.Substring(0, fileRule.Path.IndexOf("*.*"));
                            if (currentPath.IndexOf(GetPath(parentPath)) == 0 || parentPath == "")
                            {
                                FilePermission = UpdateFileRules(FilePermission, fileRule);
                            }
                        }
                        else if (fileRule.Path.IndexOf("*.") > -1)
                        {
                            var pathExtension = Path.GetExtension(fileRule.Path).ToLower();
                            var parentPath = fileRule.Path.Substring(
                                0, fileRule.Path.IndexOf("*.")
                            );
                            if ((GetPath(parentPath) == currentPath || parentPath == "")
                                && nameExtension == pathExtension)
                            {
                                FilePermission = UpdateFileRules(FilePermission, fileRule);
                            }
                        }
                        else if (fileRule.Path.IndexOf(".*") > -1)
                        {
                            var pathName = Path.GetFileNameWithoutExtension(fileRule.Path);
                            var parentPath = fileRule.Path.Substring(0, fileRule.Path.IndexOf(pathName + ".*"));
                            if ((GetPath(parentPath) == currentPath || parentPath == "") && fileName == pathName)
                            {
                                FilePermission = UpdateFileRules(FilePermission, fileRule);
                            }
                        }
                        else if (GetPath(fileRule.Path) == GetValidPath(location + name))
                        {
                            FilePermission = UpdateFileRules(FilePermission, fileRule);
                        }
                    }
                }
                return FilePermission;
            }
            else
            {
                if (_accessDetails.AccessRules == null) { return null; }
                foreach (var folderRule in _accessDetails.AccessRules)
                {
                    if (folderRule.Path != null
                        && folderRule.IsFile == false
                        && (folderRule.Role == null
                            || folderRule.Role == _accessDetails.Role
                        )
                    )
                    {
                        if (folderRule.Path.IndexOf("*") > -1)
                        {
                            var parentPath = folderRule.Path.Substring(
                                0,
                                folderRule.Path.IndexOf("*")
                            );
                            var validPathStartsWithParentPathOrParentPathEmpty = GetValidPath(location + name).IndexOf(
                                GetPath(parentPath)
                            ) == 0 || parentPath == "";
                            if (validPathStartsWithParentPathOrParentPathEmpty)
                            {
                                FilePermission = UpdateFolderRules(FilePermission, folderRule);
                            }
                        }
                        else if (GetPath(folderRule.Path) == GetValidPath(location + name)
                            || GetPath(folderRule.Path) == GetValidPath(location + name + Path.DirectorySeparatorChar)
                        )
                        {
                            FilePermission = UpdateFolderRules(FilePermission, folderRule);
                        }
                        else if (GetValidPath(location + name).IndexOf(GetPath(folderRule.Path)) == 0)
                        {
                            FilePermission.Write = HasPermission(folderRule.WriteContents);
                            FilePermission.WriteContents = HasPermission(folderRule.WriteContents);
                        }
                    }
                }
                return FilePermission;
            }
        }

        private static string GetFilePath(string path)
        {
            return Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
        }

        private static AccessPermission UpdateFileRules(

            AccessPermission filePermission,
            AccessRule fileRule
        )
        {
            filePermission.Copy = HasPermission(fileRule.Copy);
            filePermission.Download = HasPermission(fileRule.Download);
            filePermission.Write = HasPermission(fileRule.Write);
            filePermission.Read = HasPermission(fileRule.Read);
            filePermission.Message = string.IsNullOrEmpty(fileRule.Message)
                ? string.Empty
                : fileRule.Message;
            return filePermission;
        }

        private static bool HasPermission(Permission rule)
        {
            return rule == Permission.Allow;
        }

        private static string GetValidPath(string path)
        {
            return new DirectoryInfo(path).FullName;
        }

        private static AccessPermission UpdateFolderRules(
            AccessPermission folderPermission,
            AccessRule folderRule
        )
        {
            folderPermission.Copy = HasPermission(folderRule.Copy);
            folderPermission.Download = HasPermission(folderRule.Download);
            folderPermission.Write = HasPermission(folderRule.Write);
            folderPermission.WriteContents = HasPermission(folderRule.WriteContents);
            folderPermission.Read = HasPermission(folderRule.Read);
            folderPermission.Upload = HasPermission(folderRule.Upload);
            folderPermission.Message = string.IsNullOrEmpty(folderRule.Message)
                ? string.Empty
                : folderRule.Message;
            return folderPermission;
        }

        private static string GetFileNameFromPath(string path)
        {
            int index = path.LastIndexOf("/");
            return path.Substring(index + 1);
        }

        private static bool IsDirectory(
            string path,
            string fileName
        )
        {
            var fullPath = Path.Combine(
                path,
                fileName
            );
            return (File.GetAttributes(fullPath) & FileAttributes.Directory)
                == FileAttributes.Directory;
        }


        protected virtual string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        private List<FileInfo> GetDirectoryFiles(DirectoryInfo dir, List<FileInfo> files)
        {
            try
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    files = GetDirectoryFiles(subdir, files);
                }
                foreach (FileInfo file in dir.GetFiles())
                {
                    files.Add(file);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore
            }
            catch (Exception)
            {
                throw;
            }
            return files;
        }

        private List<DirectoryInfo> GetDirectoryFolders(DirectoryInfo dir, List<DirectoryInfo> files)
        {
            try
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    files = GetDirectoryFolders(subdir, files);
                }
                foreach (DirectoryInfo file in dir.GetDirectories())
                {
                    files.Add(file);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore
            }
            catch (Exception)
            {
                throw;
            }
            return files;
        }

        protected virtual bool ParentsHavePermission(FileSystemDirectoryContent fileDetails)
        {
            var parentPath = fileDetails.FilterPath?.Replace(
                Path.DirectorySeparatorChar,
                '/'
            ) ?? string.Empty;
            var parents = parentPath.Split('/');
            var currPath = "/";
            var hasPermission = true;
            for (int i = 0; i <= parents.Length - 2; i++)
            {
                currPath = (parents[i] == "") ? currPath : (currPath + parents[i] + "/");
                var PathPermission = GetPathPermission(currPath);
                if (PathPermission == null)
                {
                    break;
                }
                else if (PathPermission != null
                    && !PathPermission.Read
                )
                {
                    hasPermission = false;
                    break;
                }
            }
            return hasPermission;
        }
    }
}
