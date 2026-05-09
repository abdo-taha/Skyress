using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Items;
using Skyress.Application.Items.Commands.UpdateItemDescription;
using Skyress.Application.Items.Commands.UpdateItemName;
using Skyress.Application.Items.Commands.UpdateItemPrice;
using Skyress.Application.Items.Commands.UpdateItemQrCode;
using Skyress.Application.Items.Commands.UpdateItemQuantityLeft;
using Skyress.Application.Items.Commands.UpdateItemUnit;
using Skyress.Application.Items.Responses;

namespace Skyress.API.Endpoints.Items;

public static class UpdateItemEndpoints
{
    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateItemNameAsync(
        long id,
        UpdateItemNameRequest request,
        ISender sender)
    {
        var command = new UpdateItemNameCommand(id, request.Name);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }

    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateItemDescriptionAsync(
        long id,
        UpdateItemDescriptionRequest request,
        ISender sender)
    {
        var command = new UpdateItemDescriptionCommand(id, request.Description);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }

    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateItemPriceAsync(
        long id,
        UpdateItemPriceRequest request,
        ISender sender)
    {
        var command = new UpdateItemPriceCommand(id, request.Price, request.CostPrice);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }

    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateItemQuantityLeftAsync(
        long id,
        UpdateItemQuantityLeftRequest request,
        ISender sender)
    {
        var command = new UpdateItemQuantityLeftCommand(id, request.QuantityLeft);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }

    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateItemQrCodeAsync(
        long id,
        UpdateItemQrCodeRequest request,
        ISender sender)
    {
        var command = new UpdateItemQrCodeCommand(id, request.QrCode);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }

    public static async Task<Results<Ok<ItemResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateItemUnitAsync(
        long id,
        UpdateItemUnitRequest request,
        ISender sender)
    {
        var command = new UpdateItemUnitCommand(id, request.Unit);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }
}
