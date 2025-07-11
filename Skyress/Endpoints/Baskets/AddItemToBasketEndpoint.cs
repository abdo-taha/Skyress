using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Baskets;
using Skyress.Application.Baskets.Commands.AddItemToBasket;

namespace Skyress.API.Endpoints.Baskets;

public static class AddItemToBasketEndpoint
{
    public static async Task<IResult> AddItemToBasketAsync(
        [FromRoute] long id,
        [FromBody] AddItemToBasketRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AddItemToBasketCommand(id, request.ItemId, request.Quantity), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }
}