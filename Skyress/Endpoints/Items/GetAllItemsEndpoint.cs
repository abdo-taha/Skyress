using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Items.Queries.GetAllItems;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.API.Endpoints.Items;

public static class GetAllItemsEndpoint
{
    public static async Task<Results<Ok<IReadOnlyList<Item>>, NotFound, BadRequest<string>>> GetAllItemsAsync(
        ISender sender)
    {
        var result = await sender.Send(new GetAllItemsQuery());
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code == "GetAllItems.NotFound" 
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }
}