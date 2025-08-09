using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.CheckOutBasket;
using Skyress.API.DTOs.Baskets;

namespace Skyress.API.Endpoints.Baskets;

public static class CheckOutBasketEndpoint
{
    public static async Task<IResult> CheckOutBasketAsync(
        ISender sender,
        [FromBody] CheckOutBasketRequest request,
        CancellationToken cancellationToken)
    {
        var idempotencyKey = request.IdempotencyKey ?? Guid.NewGuid();
        
        var command = new CheckOutBasketCommand(request.BasketId, idempotencyKey);
        var result = await sender.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Results.Ok(new { IdempotencyKey = idempotencyKey });
        }
        
        return Results.BadRequest(result.Error);
    }
}
