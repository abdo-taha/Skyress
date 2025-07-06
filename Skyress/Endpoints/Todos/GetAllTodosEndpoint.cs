using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Todos.Queries.GetAllTodos;
using Skyress.Domain.Aggregates.Todo;

namespace Skyress.API.Endpoints.Todos;

public static class GetAllTodosEndpoint
{
    public static async Task<Ok<List<Todo>>> GetAllTodosAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllTodosQuery(), cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}