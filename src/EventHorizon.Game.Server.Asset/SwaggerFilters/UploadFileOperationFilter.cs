namespace EventHorizon.Game.Server.Asset.SwaggerFilters
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    public class UploadFileOperationFilter 
        : IOperationFilter
    {
        public void Apply(
            OpenApiOperation operation,
            OperationFilterContext context
        )
        {
            var uploadFileMimeType = "file-upload/upload";
            var fileUploadMime = "multipart/form-data";
            if (operation.RequestBody == null
                || !operation.RequestBody.Content.Any(
                    x => x.Key.Equals(
                        uploadFileMimeType,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                return;
            }

            var fileParams = context.MethodInfo
                .GetParameters()
                .Where(
                    parameterInfo => parameterInfo.ParameterType == typeof(IFormFile)
                );
            operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                fileParams.ToDictionary(
                    parameterInfo => parameterInfo.Name!,
                    parameterInfo => new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "binary"
                    }
                );
            operation.RequestBody
                .Content[fileUploadMime]
                .Schema.Properties.Add(
                    "file-path", 
                    new OpenApiSchema
                    {
                        Type = "string",
                    }
                );
            operation.RequestBody
                .Content[fileUploadMime]
                .Schema.Properties.Add(
                    "action", 
                    new OpenApiSchema
                    {
                        Type = "string",
                        Description = "The type of file upload to be done. [save,replace,keepboth]",
                    }
                );
        }
    }
}
