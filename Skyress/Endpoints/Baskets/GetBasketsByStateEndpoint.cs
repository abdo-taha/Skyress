using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Baskets.Queries.GetBasketsByState;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Baskets;

public static class GetBasketsByStateEndpoint
{
    public static async Task<Results<Ok<List<Basket>>, BadRequest<string>>> GetBasketsByStateAsync(
        BasketState state,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBasketsByStateQuery(state), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value.ToList());
    }
} 