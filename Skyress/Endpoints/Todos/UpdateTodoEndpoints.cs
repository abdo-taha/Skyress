using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Todos;
using Skyress.Application.Todos.Commands.UpdateTodoContext;
using Skyress.Application.Todos.Commands.UpdateTodoState;

namespace Skyress.API.Endpoints.Todos;

public static class UpdateTodoEndpoints
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> UpdateTodoStateAsync(
        long id,
        UpdateTodoStateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTodoStateCommand(id, request.State), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        
        return TypedResults.Ok();
    }
    
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> UpdateTodoContextAsync(
        long id,
        UpdateTodoContextRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTodoContextCommand(id, request.Context), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        
        return TypedResults.Ok();
    }
}