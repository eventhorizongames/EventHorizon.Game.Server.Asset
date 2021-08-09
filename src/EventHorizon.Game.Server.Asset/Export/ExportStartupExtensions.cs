﻿namespace EventHorizon.Game.Server.Asset.Export
{
    using System.IO;

    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.Export.Api;
    using EventHorizon.Game.Server.Asset.Export.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;

    public static class ExportStartupExtensions
    {
        public static IServiceCollection AddExportServices(
            this IServiceCollection services
        ) => services
            .AddSingleton<ExportArtifactService, StandardExportArtifactService>()
            .AddSingleton<ExportTriggerService, StandardExportTriggerService>()
        ;

        public static IApplicationBuilder UseExport(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            AssetServerContentDirectories directories
        )
        {
            var exportsPath = Path.Combine(
                env.ContentRootPath,
                directories.DataDirectory,
                directories.ExportsDirectory
            );
            Directory.CreateDirectory(
                exportsPath
            );
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        env.ContentRootPath,
                        directories.DataDirectory,
                        directories.ExportsDirectory
                    )
                ),
                RequestPath = $"/{directories.ExportsDirectory}",
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        env.ContentRootPath,
                        directories.DataDirectory,
                        directories.ExportsDirectory
                    )
                ),
                RequestPath = $"/{directories.ExportsDirectory}",
            });

            return app;
        }
    }
}
