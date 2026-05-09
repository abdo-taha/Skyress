using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Items.Queries.GetAllItems;
using Skyress.Application.Items.Responses;

namespace Skyress.API.Endpoints.Items;

public static class GetAllItemsEndpoint
{
    public static async Task<Results<Ok<IReadOnlyList<ItemResponse>>, NotFound, UnprocessableEntity<ProblemDetails>>> GetAllItemsAsync(
        ISender sender)
    {
        var result = await sender.Send(new GetAllItemsQuery());
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code == "GetAllItems.NotFound"
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }
}
