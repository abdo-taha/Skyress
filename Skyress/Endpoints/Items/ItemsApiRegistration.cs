using Asp.Versioning.Conventions;
using Skyress.API.Endpoints.Items;

namespace Skyress.API.Endpoints.Items;

public static class ItemsApiRegistration
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
        
        // Query endpoints
        api.MapGet("/", GetAllItemsEndpoint.GetAllItemsAsync);
        api.MapGet("{id:long}", GetItemByIdEndpoint.GetItemByIdAsync);
        api.MapGet("/pricingHistory{id:long}", GetItemPricingHistoryEndpoint.GetItemPricingHistoryAsync);
        
        // Command endpoints
        api.MapPost("/", CreateItemEndpoint.CreateItemAsync);
        api.MapDelete("{id:long}", DeleteItemEndpoint.DeleteItemAsync);

        // Update endpoints
        api.MapPatch("{id:long}/name", UpdateItemEndpoints.UpdateItemNameAsync);
        api.MapPatch("{id:long}/description", UpdateItemEndpoints.UpdateItemDescriptionAsync);
        api.MapPatch("{id:long}/price", UpdateItemEndpoints.UpdateItemPriceAsync);
        api.MapPatch("{id:long}/costPrice", UpdateItemEndpoints.UpdateItemCostPriceAsync);
        api.MapPatch("{id:long}/quantityLeft", UpdateItemEndpoints.UpdateItemQuantityLeftAsync);
        api.MapPatch("{id:long}/qrcode", UpdateItemEndpoints.UpdateItemQrCodeAsync);
        api.MapPatch("{id:long}/unit", UpdateItemEndpoints.UpdateItemUnitAsync);
    }
    
    private static RouteGroupBuilder MapItemsApiV2(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(2.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/items").WithApiVersionSet(versionSet);
        api.MapGet("{id:long}", GetItemByIdEndpoint.GetItemByIdAsync);
    
        return api;
    }
}