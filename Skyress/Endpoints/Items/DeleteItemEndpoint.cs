using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Items.Commands.DeleteItem;

namespace Skyress.API.Endpoints.Items;

public static class DeleteItemEndpoint
{
    public static async Task<Results<NoContent, NotFound, UnprocessableEntity<ProblemDetails>>> DeleteItemAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new DeleteItemCommand(id));
        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.Code == "DeleteItem.NotFound"
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }
}
