using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Todos.Queries.GetTodosByState;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Todos;

public static class GetTodosByStateEndpoint
{
    public static async Task<Results<Ok<List<Todo>>, BadRequest<string>>> GetTodosByStateAsync(
        TodoState state,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTodosByStateQuery(state), cancellationToken);
        
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
            
        return TypedResults.Ok(result.Value);
    }
}