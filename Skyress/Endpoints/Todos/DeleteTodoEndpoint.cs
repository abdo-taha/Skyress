using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Todos.Commands.DeleteTodo;

namespace Skyress.API.Endpoints.Todos;

public static class DeleteTodoEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> DeleteTodoAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTodoCommand(id), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        
        return TypedResults.Ok();
    }
}