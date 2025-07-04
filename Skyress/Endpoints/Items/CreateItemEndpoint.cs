using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Items;
using Skyress.Application.Items.Commands.CreateItem;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.API.Endpoints.Items;

public static class CreateItemEndpoint
{
    public static async Task<Results<Ok<Item>, BadRequest<string>>> CreateItemAsync(
        CreateItemRequest request,
        ISender sender)
    {
        var command = new CreateItemCommand(
            request.Name,
            request.Description,
            request.Price,
            request.Unit,
            request.CostPrice,
            request.QuantityLeft,
            request.QrCode
        );
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Error.Message);
    }
}