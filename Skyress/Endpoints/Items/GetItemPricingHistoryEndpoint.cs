using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Items.Queries.GetItemPricingHistory;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.API.Endpoints.Items;

public static class GetItemPricingHistoryEndpoint
{
    public static async Task<Results<Ok<List<PricingHistory> >, NotFound<string>>> GetItemPricingHistoryAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new GetItemPricingHistoryQuery(id));
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.NotFound(result.Error.Message);
    }
}