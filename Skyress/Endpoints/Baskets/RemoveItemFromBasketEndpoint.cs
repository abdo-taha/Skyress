using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Baskets;
using Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

namespace Skyress.API.Endpoints.Baskets;

public static class RemoveItemFromBasketEndpoint
{
    public static async Task<IResult> RemoveItemFromBasketAsync(
        ISender sender,
        [FromRoute] long id,
        [FromBody] RemoveItemFromBasketRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveItemFromBasketCommand(id, request.ItemId), cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
}