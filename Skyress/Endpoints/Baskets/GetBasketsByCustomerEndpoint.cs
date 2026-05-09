using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Baskets.Queries.GetBasketsByCustomer;
using Skyress.Application.Baskets.Responses;

namespace Skyress.API.Endpoints.Baskets;

public static class GetBasketsByCustomerEndpoint
{
    public static async Task<Ok<IReadOnlyList<BasketResponse>>> GetBasketsByCustomerAsync(
        long? customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBasketsByCustomerQuery(customerId), cancellationToken);
        return TypedResults.Ok(result.Value);
    }
} 