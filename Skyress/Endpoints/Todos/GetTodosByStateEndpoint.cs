using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Todos.Queries.GetTodosByState;
using Skyress.Application.Todos.Responses;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Todos;

public static class GetTodosByStateEndpoint
{
    public static async Task<Results<Ok<IReadOnlyList<TodoResponse>>, UnprocessableEntity<ProblemDetails>>> GetTodosByStateAsync(
        TodoState state,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTodosByStateQuery(state), cancellationToken);

        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });

        return TypedResults.Ok(result.Value as IReadOnlyList<TodoResponse>);
    }
}