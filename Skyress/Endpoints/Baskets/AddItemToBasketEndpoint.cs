using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Baskets;
using Skyress.Application.Baskets.Commands.AddItemToBasket;
using Skyress.Application.Baskets.Responses;

namespace Skyress.API.Endpoints.Baskets;

public static class AddItemToBasketEndpoint
{
    public static async Task<Results<Ok<BasketResponse>, UnprocessableEntity<ProblemDetails>>> AddItemToBasketAsync(
        [FromRoute] long id,
        [FromBody] AddItemToBasketRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AddItemToBasketCommand(id, request.ItemId, request.Quantity), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(BasketResponse.FromDomain(result.Value))
            : TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
    }
}