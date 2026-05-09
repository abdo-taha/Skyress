using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Todos.Commands.DeleteTodo;

namespace Skyress.API.Endpoints.Todos;

public static class DeleteTodoEndpoint
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> DeleteTodoAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTodoCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }

        return TypedResults.Ok();
    }
}