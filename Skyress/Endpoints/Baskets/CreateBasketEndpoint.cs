using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Baskets;
using Skyress.Application.Baskets.Commands.CreateBasket;

namespace Skyress.API.Endpoints.Baskets;

public static class CreateBasketEndpoint
{
    public static async Task<IResult> CreateBasketAsync(
        ISender sender,
        [FromBody] CreateBasketRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateBasketCommand(request.CustomerId), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
    }
}