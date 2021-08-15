namespace EventHorizon.Game.Server.Asset
{
    using System;
    using System.IO;
    using System.Linq;

    using EventHorizon.BackgroundTasks;
    using EventHorizon.Game.Server.Asset.Core.Api;
    using EventHorizon.Game.Server.Asset.Core.Model;
    using EventHorizon.Game.Server.Asset.Export;
    using EventHorizon.Game.Server.Asset.FileManagement.Api;
    using EventHorizon.Game.Server.Asset.FileManagement.Providers;
    using EventHorizon.Game.Server.Asset.Hub.Base;
    using EventHorizon.Game.Server.Asset.Import;
    using EventHorizon.Game.Server.Asset.Policies;
    using EventHorizon.Game.Server.Asset.SwaggerFilters;

    using MediatR;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.Filters;

    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env
        )
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Game Development Platform - Asset Server",
                        Version = "V1",
                        Description = "",
                        TermsOfService = new Uri("https://ehzgames.studio/terms-of-service"),
                        Contact = new OpenApiContact
                        {
                            Name = "Cody Merritt Anhorn",
                            Email = "cody.anhorn+asset_server@hotmail.com",
                            Url = new Uri("https://codyanhorn.tech"),
                        },
                    });

                    options.AddSecurityDefinition(
                        "oauth2",
                        new OpenApiSecurityScheme()
                        {
                            Description = "Authorization header using the Bearer scheme.",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                        }
                    );

                    options.OperationFilter<UploadFileOperationFilter>();
                    options.OperationFilter<SecurityRequirementsOperationFilter>();
                }
            );

            services.AddControllers();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.RequireHttpsMetadata = HostingEnvironment.IsProduction() || HostingEnvironment.IsStaging();
                    options.Authority = Configuration["Auth:Authority"];
                    options.ApiName = Configuration["Auth:ApiName"];
                    options.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Jwt;

                    options.TokenRetriever = WebSocketTokenRetriever.FromHeaderAndQueryString;
                });
            services.AddAuthorization(
                options => options.AddUserIdOrAdminPolicy(
                    Configuration["OwnerDetails:UserId"]
                )
            );

            services.AddCors(
                options => options.AddPolicy(
                    "CorsPolicy",
                    builder =>
                    {
                        builder.AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(
                                Configuration
                                    .GetSection(
                                        "Cors:Hosts"
                                    ).GetChildren()
                                    .AsEnumerable()
                                    .Select(
                                        a => a.Value
                                    ).ToArray()
                            ).AllowCredentials();
                    }
                )
            );

            services.AddDirectoryBrowser();

            services.AddSignalR();

            services.AddMediatR(
                new Type[]
                {
                    typeof(Startup),

                    typeof(BackgroundTasksStartupExtensions),
                }
            );

            services.AddSingleton<AssetServerContentDirectories, StaticAssetServerContentDirectories>();

            services.AddTransient<FileSystemProvider, PhysicalFileSystemProvider>();

            services.AddExportServices();

            services.AddImportServices();

            services.AddBackgroundTasksServices();

        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            AssetServerContentDirectories directories
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".fx"] = "application/fx";
            provider.Mappings[".gltf"] = "model/vnd.gltf+json";
            provider.Mappings[".glb"] = "application/octet-stream";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = provider,
                RequestPath = $"/{directories.AssetsDirectory}",
            });
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        env.ContentRootPath,
                        directories.AssetDirectory
                    )
                ),
                RequestPath = $"/{directories.AssetsDirectory}",
            });

            app.UseExport(
                env,
                directories
            );

            app.UseImport(
                env,
                directories
            );

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapHub<AdminHub>("/admin");

                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<h1>Hello World!</h1>");
                    await context.Response.WriteAsync("<a href=\"/swagger\">Swagger API</a>");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync("<a href=\"/Assets\">Assets</a>");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync($"<a href=\"/{directories.ExportsDirectory}\">Exports</a>");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync($"<a href=\"/{directories.ImportsDirectory}\">Imports</a>");
                });
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "EventHorizon Asset Server API V1");
            });
        }
    }
}
