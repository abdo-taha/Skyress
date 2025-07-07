using Asp.Versioning.Conventions;

namespace Skyress.API.Endpoints.TagAssignments;

public static class TagAssignmentApiRegistration
{
    public static void MapTagAssignmentsApi(this IEndpointRouteBuilder app)
    {
        app.MapTagAssignmentsApiV1();
    }

    private static void MapTagAssignmentsApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/tagAssignments").WithApiVersionSet(versionSet).WithTags("TagAssignments");
        

        api.MapGet("/item/{itemId:long}", GetTagAssignmentsByItemEndpoint.GetTagAssignmentsByItemAsync);
        api.MapGet("/tag/{tagId:long}", GetTagAssignmentsByTagEndpoint.GetTagAssignmentsByTagAsync);
        

        api.MapPost("/", CreateTagAssignmentEndpoint.CreateTagAssignmentAsync);
        api.MapDelete("{id:long}", DeleteTagAssignmentEndpoint.DeleteTagAssignmentAsync);
    }
}