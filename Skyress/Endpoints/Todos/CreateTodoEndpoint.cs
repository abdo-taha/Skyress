using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Todos;
using Skyress.Application.Todos.Commands.CreateTodo;
using Skyress.Application.Todos.Responses;

namespace Skyress.API.Endpoints.Todos;

public static class CreateTodoEndpoint
{
    public static async Task<Results<Ok<TodoResponse>, UnprocessableEntity<ProblemDetails>>> CreateTodoAsync(
        CreateTodoRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTodoCommand(request.Context), cancellationToken);

        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });

        return TypedResults.Ok(result.Value);
    }
}