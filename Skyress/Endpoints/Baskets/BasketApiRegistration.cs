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

        app.MapPost("/", CreateBasketEndpoint.CreateBasketAsync);

        app.MapGet("{id:long}", GetBasketByIdEndpoint.GetBasketByIdAsync);

        app.MapPost("{id:long}/items", AddItemToBasketEndpoint.AddItemToBasketAsync);

        app.MapGet("/customer/{customerId:long}", GetBasketsByCustomerEndpoint.GetBasketsByCustomerAsync);
        app.MapGet("/state/{state}", GetBasketsByStateEndpoint.GetBasketsByStateAsync);

        app.MapDelete("{id:long}", ClearBasketEndpoint.ClearBasketAsync);

        app.MapDelete("{id:long}/items", RemoveItemFromBasketEndpoint.RemoveItemFromBasketAsync);


        app.MapPost("{id:long}/cancel-reservation", CancelBasketReservationEndpoint.CancelBasketReservationAsync);

        app.MapPatch("checkout", CheckOutBasketEndpoint.CheckOutBasketAsync);
    }
}