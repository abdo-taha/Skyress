using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Baskets.Queries.GetBasketById;
using Skyress.Application.Baskets.Responses;

namespace Skyress.API.Endpoints.Baskets;

public static class GetBasketByIdEndpoint
{
    public static async Task<Results<Ok<BasketResponse>, NotFound>> GetBasketByIdAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBasketByIdQuery(id), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.NotFound();
    }
}