using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Items.Queries.GetItemById;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.API.Endpoints.Items;

public static class GetItemByIdEndpoint
{
    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> GetItemByIdAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new GetItemByIdQuery(id));
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Error.Message);
    }
}