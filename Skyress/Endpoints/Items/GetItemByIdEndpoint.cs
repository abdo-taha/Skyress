using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Items.Queries.GetItemById;
using Skyress.Application.Items.Responses;

namespace Skyress.API.Endpoints.Items;

public static class GetItemByIdEndpoint
{
    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> GetItemByIdAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new GetItemByIdQuery(id));
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
