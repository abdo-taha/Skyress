using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Baskets;
using Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

namespace Skyress.API.Endpoints.Baskets;

public static class RemoveItemFromBasketEndpoint
{
    public static async Task<Results<Ok, UnprocessableEntity<ProblemDetails>>> RemoveItemFromBasketAsync(
        ISender sender,
        [FromRoute] long id,
        [FromBody] RemoveItemFromBasketRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveItemFromBasketCommand(id, request.ItemId), cancellationToken);
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