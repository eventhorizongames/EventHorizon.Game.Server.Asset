namespace EventHorizon.Game.Server.Asset.Common.Backup;
using System.IO;

using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Common.Backup.Api;
using EventHorizon.Game.Server.Asset.Common.Backup.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

public static class CommonBackupStartupExtensions
{
    public static IServiceCollection AddCommonBackupServices(
        this IServiceCollection services
    ) => services
        .AddSingleton<BackupArtifactService, BackupArtifactFileSystemService>()
    ;

    public static IApplicationBuilder UseCommonBackup(
        this IApplicationBuilder app,
        IWebHostEnvironment env,
        AssetServerSettings settings
    )
    {
        var backupsPath = Path.Combine(
            env.ContentRootPath,
            settings.DataDirectory,
            settings.BackupsDirectory
        );
        Directory.CreateDirectory(
            backupsPath
        );
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    env.ContentRootPath,
                    settings.DataDirectory,
                    settings.BackupsDirectory
                )
            ),
            RequestPath = $"/{settings.BackupsDirectory}",
        });
        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    env.ContentRootPath,
                    settings.DataDirectory,
                    settings.BackupsDirectory
                )
            ),
            RequestPath = $"/{settings.BackupsDirectory}",
        });

        return app;
    }
}
