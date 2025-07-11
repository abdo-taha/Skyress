using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.ClearBasket;

namespace Skyress.API.Endpoints.Baskets;

public static class ClearBasketEndpoint
{
    public static async Task<IResult> ClearBasketAsync(
        ISender sender,
        [FromRoute] long basketId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ClearBasketCommand(basketId), cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
}