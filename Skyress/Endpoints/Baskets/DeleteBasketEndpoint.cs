using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.DeleteBasketCommand;

namespace Skyress.API.Endpoints.Baskets;

public static class DeleteBasketEndpoint
{
    public static async Task<Results<Ok, UnprocessableEntity<ProblemDetails>>> DeleteBasketAsync(
        ISender sender,
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteBasketCommand(id), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
    }
}