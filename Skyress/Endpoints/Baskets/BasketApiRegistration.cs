using Asp.Versioning.Conventions;

namespace Skyress.API.Endpoints.Baskets;

public static class BasketApiRegistration
{
    public static void MapBasketsApi(this IEndpointRouteBuilder app)
    {
        app.MapBasketsApiV1();
    }
    public static void MapBasketsApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/baskets").WithApiVersionSet(versionSet).WithTags("Baskets");

        api.MapPost("/", CreateBasketEndpoint.CreateBasketAsync);

        api.MapGet("{id:long}", GetBasketByIdEndpoint.GetBasketByIdAsync);

        api.MapPost("{id:long}/items", AddItemToBasketEndpoint.AddItemToBasketAsync);

        api.MapGet("/customer/{customerId:long}", GetBasketsByCustomerEndpoint.GetBasketsByCustomerAsync);
        api.MapGet("/state/{state}", GetBasketsByStateEndpoint.GetBasketsByStateAsync);

        api.MapDelete("{id:long}", ClearBasketEndpoint.ClearBasketAsync);

        api.MapDelete("{id:long}/items", RemoveItemFromBasketEndpoint.RemoveItemFromBasketAsync);


        api.MapPost("{id:long}/cancel-reservation", CancelBasketReservationEndpoint.CancelBasketReservationAsync);

        api.MapPatch("checkout", CheckOutBasketEndpoint.CheckOutBasketAsync);
    }
}