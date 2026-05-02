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
        var api = app.MapGroup("api/items").WithApiVersionSet(versionSet).WithTags("Items");
        
        // Query endpoints
        api.MapGet("/", GetAllItemsEndpoint.GetAllItemsAsync);
        api.MapGet("{id:long}", GetItemByIdEndpoint.GetItemByIdAsync);
        api.MapGet("/pricingHistory{id:long}", GetItemPricingHistoryEndpoint.GetItemPricingHistoryAsync);
        
        // Command endpoints — Admin only
        api.MapPost("/", CreateItemEndpoint.CreateItemAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapDelete("{id:long}", DeleteItemEndpoint.DeleteItemAsync).RequireAuthorization(p => p.RequireRole("Admin"));

        // Update endpoints — Admin only
        api.MapPatch("{id:long}/name", UpdateItemEndpoints.UpdateItemNameAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/description", UpdateItemEndpoints.UpdateItemDescriptionAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/price", UpdateItemEndpoints.UpdateItemPriceAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/quantityLeft", UpdateItemEndpoints.UpdateItemQuantityLeftAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/qrcode", UpdateItemEndpoints.UpdateItemQrCodeAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/unit", UpdateItemEndpoints.UpdateItemUnitAsync).RequireAuthorization(p => p.RequireRole("Admin"));
    }
    
    private static RouteGroupBuilder MapItemsApiV2(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(2.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/items").WithApiVersionSet(versionSet).WithTags("Items");
        api.MapGet("{id:long}", GetItemByIdEndpoint.GetItemByIdAsync);
    
        return api;
    }
}