namespace EventHorizon.Game.Server.Asset.Common.Export;
using System.IO;

using EventHorizon.Game.Server.Asset.Core.Api;
using EventHorizon.Game.Server.Asset.Common.Export.Api;
using EventHorizon.Game.Server.Asset.Common.Export.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

public static class CommonExportStartupExtensions
{
    public static IServiceCollection AddCommonExportServices(
        this IServiceCollection services
    ) => services
        .AddSingleton<ExportArtifactService, ExportArtifactFileSystemService>()
    ;

    public static IApplicationBuilder UseCommonExport(
        this IApplicationBuilder app,
        IWebHostEnvironment env,
        AssetServerSettings settings
    )
    {
        var exportsPath = Path.Combine(
            env.ContentRootPath,
            settings.DataDirectory,
            settings.ExportsDirectory
        );
        Directory.CreateDirectory(
            exportsPath
        );
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    env.ContentRootPath,
                    settings.DataDirectory,
                    settings.ExportsDirectory
                )
            ),
            RequestPath = $"/{settings.ExportsDirectory}",
        });
        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(
                    env.ContentRootPath,
                    settings.DataDirectory,
                    settings.ExportsDirectory
                )
            ),
            RequestPath = $"/{settings.ExportsDirectory}",
        });

        return app;
    }
}
