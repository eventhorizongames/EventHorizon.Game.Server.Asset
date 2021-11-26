namespace EventHorizon.Game.Server.Asset.Common.Import;
using System.IO;

using EventHorizon.Game.Server.Asset.Common.Import.Api;
using EventHorizon.Game.Server.Asset.Common.Import.Services;
using EventHorizon.Game.Server.Asset.Core.Api;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

public static class CommonImportStartupExtensions
{
    public static IServiceCollection AddCommonImportServices(
        this IServiceCollection services
    ) => services
        .AddSingleton<ImportArtifactService, ImportArtifactFileSystemService>()
    ;

    public static IApplicationBuilder UseCommonImport(
        this IApplicationBuilder app,
        IWebHostEnvironment env,
        AssetServerSettings settings
    )
    {
        var importsPath = Path.Combine(
            env.ContentRootPath,
            settings.DataDirectory,
            settings.ImportsDirectory
        );
        Directory.CreateDirectory(
            importsPath
        );
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    env.ContentRootPath,
                    settings.DataDirectory,
                    settings.ImportsDirectory
                )
            ),
            RequestPath = $"/{settings.ImportsDirectory}",
        });
        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    env.ContentRootPath,
                    settings.DataDirectory,
                    settings.ImportsDirectory
                )
            ),
            RequestPath = $"/{settings.ImportsDirectory}",
        });

        return app;
    }
}
