using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Todos;
using Skyress.Application.Todos.Commands.CreateTodo;
using Skyress.Domain.Aggregates.Todo;

namespace Skyress.API.Endpoints.Todos;

public static class CreateTodoEndpoint
{
    public static async Task<Results<Ok<Todo>, BadRequest<string>>> CreateTodoAsync(
        CreateTodoRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTodoCommand(request.Context), cancellationToken);
        
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
            
        return TypedResults.Ok(result.Value);
    }
}