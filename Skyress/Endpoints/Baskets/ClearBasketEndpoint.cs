using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.ClearBasket;

namespace Skyress.API.Endpoints.Baskets;

public static class ClearBasketEndpoint
{
    public static async Task<Results<Ok, UnprocessableEntity<ProblemDetails>>> ClearBasketAsync(
        ISender sender,
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ClearBasketCommand(id), cancellationToken);
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