using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Baskets.Queries.GetBasketsByCustomer;
using Skyress.Domain.Aggregates.Basket;

namespace Skyress.API.Endpoints.Baskets;

public static class GetBasketsByCustomerEndpoint
{
    public static async Task<Ok<List<Basket>>> GetBasketsByCustomerAsync(
        long customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBasketsByCustomerQuery(customerId), cancellationToken);
        return TypedResults.Ok(result.Value.ToList());
    }
} 