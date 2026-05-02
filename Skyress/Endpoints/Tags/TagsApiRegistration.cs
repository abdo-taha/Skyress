using Asp.Versioning.Conventions;

namespace Skyress.API.Endpoints.Tags;

public static class TagsApiRegistration
{
    public static void MapTagsApi(this IEndpointRouteBuilder app)
    {
        app.MapTagsApiV1();
    }

    private static void MapTagsApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/tags").WithApiVersionSet(versionSet).WithTags("Tags");
        
        api.MapGet("/", GetAllTagsEndpoint.GetAllTagsAsync);
        api.MapGet("{id:long}", GetTagEndpoint.GetTagByIdAsync);
        api.MapGet("/type/{type:int}", GetTagsByTypeEndpoint.GetTagsByTypeAsync);
        
        // Write operations — Admin only
        api.MapPost("/", CreateTagEndpoint.CreateTagAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapDelete("{id:long}", DeleteTagEndpoint.DeleteTagAsync).RequireAuthorization(p => p.RequireRole("Admin"));

        api.MapPatch("{id:long}/name", UpdateTagEndpoints.UpdateTagNameAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/type", UpdateTagEndpoints.UpdateTagTypeAsync).RequireAuthorization(p => p.RequireRole("Admin"));
    }
}