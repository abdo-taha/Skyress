using Asp.Versioning.Conventions;

namespace Skyress.API.Endpoints.Todos;

public static class TodosApiRegistration
{
    public static void MapTodosApi(this IEndpointRouteBuilder app)
    {
        app.MapTodosApiV1();
    }

    private static void MapTodosApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/todos").WithApiVersionSet(versionSet).WithTags("Todos");
        
        api.MapGet("/", GetAllTodosEndpoint.GetAllTodosAsync);
        api.MapGet("{id:long}", GetTodoEndpoint.GetTodoByIdAsync);
        api.MapGet("/state/{state}", GetTodosByStateEndpoint.GetTodosByStateAsync);
        
        api.MapPost("/", CreateTodoEndpoint.CreateTodoAsync);
        api.MapDelete("{id:long}", DeleteTodoEndpoint.DeleteTodoAsync);
        
        api.MapPatch("{id:long}/state", UpdateTodoEndpoints.UpdateTodoStateAsync);
        api.MapPatch("{id:long}/context", UpdateTodoEndpoints.UpdateTodoContextAsync);
    }
}