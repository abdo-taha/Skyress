using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Items;
using Skyress.Application.Items.Commands.CreateItem;
using Skyress.Application.Items.Responses;

namespace Skyress.API.Endpoints.Items;

public static class CreateItemEndpoint
{
    public static async Task<Results<Ok<ItemResponse>, UnprocessableEntity<ProblemDetails>>> CreateItemAsync(
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
            : TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
    }
}
