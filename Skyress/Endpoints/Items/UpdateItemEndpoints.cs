using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Items;
using Skyress.Application.Items.Commands.UpdateItemCostPrice;
using Skyress.Application.Items.Commands.UpdateItemDescription;
using Skyress.Application.Items.Commands.UpdateItemName;
using Skyress.Application.Items.Commands.UpdateItemPrice;
using Skyress.Application.Items.Commands.UpdateItemQrCode;
using Skyress.Application.Items.Commands.UpdateItemQuantityLeft;
using Skyress.Application.Items.Commands.UpdateItemUnit;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.API.Endpoints.Items;

public static class UpdateItemEndpoints
{
    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemNameAsync(
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
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemDescriptionAsync(
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
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemPriceAsync(
        long id,
        UpdateItemPriceRequest request,
        ISender sender)
    {
        var command = new UpdateItemPriceCommand(id, request.Price);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemCostPriceAsync(
        long id,
        UpdateItemCostPriceRequest request,
        ISender sender)
    {
        var command = new UpdateItemCostPriceCommand(id, request.CostPrice);
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemQuantityLeftAsync(
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
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemQrCodeAsync(
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
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemUnitAsync(
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
                : TypedResults.BadRequest(result.Error.Message);
    }
}