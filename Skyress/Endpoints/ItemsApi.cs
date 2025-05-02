namespace Skyress.API.Endpoints;

using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Items.Commands.CreateItem;
using Skyress.Application.Items.Commands.DeleteItem;
using Skyress.Application.Items.Commands.UpdateItem;
using Skyress.Application.Items.Queries.GetItemById;
using Skyress.Domain.Aggregates.Item;

// todo [AsParameters] ItemServices
public static class ItemsApi
{
    public static RouteGroupBuilder MapItemsApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/items").HasApiVersion(1.0);

        api.MapGet("{id:long}", GetItemByIdAsync);
        api.MapPost("/", CreateItemAsync);
        api.MapPut("{id:long}", UpdateItemAsync);
        api.MapDelete("{id:long}", DeleteItemAsync);

        return api;
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> GetItemByIdAsync(
        long id,
        ISender sender)
    {
        var result = await sender.Send(new GetItemByIdQuery(id));
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code == "GetItemById.NotFound"
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, BadRequest<string>>> CreateItemAsync(
        CreateItemCommand command,
        ISender sender)
    {
        var result = await sender.Send(command);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Error.Message);
    }

    public static async Task<Results<Ok<Item>, NotFound, BadRequest<string>>> UpdateItemAsync(
        long id,
        UpdateItemCommand command,
        ISender sender)
    {
        var result = await sender.Send(command with { Id = id });
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Code == "UpdateItem.NotFound"
                ? TypedResults.NotFound()
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
} 