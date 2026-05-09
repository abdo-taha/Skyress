using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Items.Queries.GetItemPricingHistory;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.API.Endpoints.Items;

public static class GetItemPricingHistoryEndpoint
{
    public static async Task<Results<Ok<List<PricingHistory>>, NotFound, UnprocessableEntity<ProblemDetails>>> GetItemPricingHistoryAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new GetItemPricingHistoryQuery(id));
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
    }
}
