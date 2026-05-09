using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Todos;
using Skyress.Application.Todos.Commands.UpdateTodoContext;
using Skyress.Application.Todos.Commands.UpdateTodoState;

namespace Skyress.API.Endpoints.Todos;

public static class UpdateTodoEndpoints
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateTodoStateAsync(
        long id,
        UpdateTodoStateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTodoStateCommand(id, request.State), cancellationToken);

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

    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateTodoContextAsync(
        long id,
        UpdateTodoContextRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTodoContextCommand(id, request.Context), cancellationToken);

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