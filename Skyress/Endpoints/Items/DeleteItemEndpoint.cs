using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Items.Commands.DeleteItem;

namespace Skyress.API.Endpoints.Items;

public static class DeleteItemEndpoint
{
    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> DeleteItemAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new DeleteItemCommand(id));
        return result.IsSuccess
            ? TypedResults.NoContent()
            : result.Error.Code == "DeleteItem.NotFound"
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }
}