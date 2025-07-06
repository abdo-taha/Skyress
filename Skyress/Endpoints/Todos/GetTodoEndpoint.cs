using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Todos.Queries.GetTodoById;
using Skyress.Domain.Aggregates.Todo;

namespace Skyress.API.Endpoints.Todos;

public static class GetTodoEndpoint
{
    public static async Task<Results<Ok<Todo>, NotFound>> GetTodoByIdAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTodoByIdQuery(id), cancellationToken);
        
        if (result.IsFailure)
            return TypedResults.NotFound();
            
        return TypedResults.Ok(result.Value);
    }
}