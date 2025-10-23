using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Baskets.Commands.InitiateCheckout;

namespace Skyress.API.Endpoints.Baskets;

public static class InitiateCheckoutBasketEndpoint
{
    public static async Task<Ok> InitiateCheckoutBasketAsync(
        long basketId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new InitiateCheckoutCommand(basketId), cancellationToken);
        return TypedResults.Ok();
    }
}