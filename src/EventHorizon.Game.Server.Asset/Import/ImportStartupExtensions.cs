namespace EventHorizon.Game.Server.Asset.Import
{
    using System.IO;

    using EventHorizon.Game.Server.Asset.Core.Api;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;

    public static class ImportStartupExtensions
    {
        public static IServiceCollection AddImportServices(
            this IServiceCollection services
        ) => services
        ;

        public static IApplicationBuilder UseImport(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            AssetServerContentDirectories directories
        )
        {
            var importsPath = Path.Combine(
                env.ContentRootPath,
                directories.DataDirectory,
                directories.ImportsDirectory
            );
            Directory.CreateDirectory(
                importsPath
            );
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        env.ContentRootPath,
                        directories.DataDirectory,
                        directories.ImportsDirectory
                    )
                ),
                RequestPath = $"/{directories.ImportsDirectory}",
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        env.ContentRootPath,
                        directories.DataDirectory,
                        directories.ImportsDirectory
                    )
                ),
                RequestPath = $"/{directories.ImportsDirectory}",
            });

            return app;
        }
    }
}
