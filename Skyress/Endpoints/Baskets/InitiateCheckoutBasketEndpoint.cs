using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.Endpoints;
using Skyress.Application.Baskets.Commands.InitiateCheckout;

namespace Skyress.API.Endpoints.Baskets;

public static class InitiateCheckoutBasketEndpoint
{
    public static async Task<Results<Ok, UnprocessableEntity<ProblemDetails>>> InitiateCheckoutBasketAsync(
        long basketId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new InitiateCheckoutCommand(basketId), cancellationToken);
        return result.ToOkOrUnprocessableEntity();
    }
}
