using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace Skyress.API.OpenApi;

using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"Skyress API {description.ApiVersion}",
                Version = description.GroupName
            });
        }
    }
}