namespace Skyress.API.Endpoints;

using Asp.Versioning.Conventions;
using Skyress.Application.Items.Queries.GetAllItems;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Items.Commands.CreateItem;
using Application.Items.Commands.DeleteItem;
using Application.Items.Commands.UpdateItemName;
using Application.Items.Commands.UpdateItemDescription;
using Application.Items.Commands.UpdateItemPrice;
using Application.Items.Commands.UpdateItemCostPrice;
using Application.Items.Commands.UpdateItemQuantityLeft;
using Application.Items.Commands.UpdateItemQrCode;
using Application.Items.Commands.UpdateItemUnit; 
using Application.Items.Queries.GetItemById;
using Domain.Aggregates.Item;

// todo [AsParameters] ItemServices
public static class ItemsApi
{
    public static void MapItemsApi(this IEndpointRouteBuilder app)
    {
        app.MapItemsApiV1();
        app.MapItemsApiV2();
    }

    private static void MapItemsApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/items").WithApiVersionSet(versionSet);
        api.MapGet("/", GetAllItemsAsync); 
        api.MapGet("{id:long}", GetItemByIdAsync);
        api.MapPost("/", CreateItemAsync);
        api.MapDelete("{id:long}", DeleteItemAsync);

        api.MapPatch("{id:long}/name", UpdateItemNameAsync);
        api.MapPatch("{id:long}/description", UpdateItemDescriptionAsync);
        api.MapPatch("{id:long}/price", UpdateItemPriceAsync);
        api.MapPatch("{id:long}/costprice", UpdateItemCostPriceAsync);
        api.MapPatch("{id:long}/quantityleft", UpdateItemQuantityLeftAsync);
        api.MapPatch("{id:long}/qrcode", UpdateItemQrCodeAsync);
        api.MapPatch("{id:long}/unit", UpdateItemUnitAsync);
    }
    
    private static RouteGroupBuilder MapItemsApiV2(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(2.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/items").WithApiVersionSet(versionSet);
        api.MapGet("{id:long}", GetItemByIdAsync);
    
        return api;
    }
    
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

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> GetItemByIdAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new GetItemByIdQuery(id));
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
                : TypedResults.BadRequest(result.Error.Message);
    }
    
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

public record CreateItemRequest(
    string Name,
    string Description,
    double Price,
    Domain.Enums.Unit Unit,
    int QuantityLeft = 0,
    double? CostPrice = null,
    string? QrCode = null
);

public record UpdateItemNameRequest(string Name);
public record UpdateItemDescriptionRequest(string Description);
public record UpdateItemPriceRequest(double Price);
public record UpdateItemCostPriceRequest(double? CostPrice);
public record UpdateItemQuantityLeftRequest(int QuantityLeft);
public record UpdateItemQrCodeRequest(string? QrCode);
public record UpdateItemUnitRequest(Domain.Enums.Unit Unit);