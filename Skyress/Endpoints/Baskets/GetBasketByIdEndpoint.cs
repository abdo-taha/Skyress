using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Queries.GetBasketById;

namespace Skyress.API.Endpoints.Baskets;

public static class GetBasketByIdEndpoint
{
    public static async Task<IResult> GetBasketByIdAsync(
        [FromRoute] long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBasketByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
    }
}