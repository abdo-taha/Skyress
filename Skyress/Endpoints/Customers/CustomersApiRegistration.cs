using Asp.Versioning.Conventions;
using Skyress.API.Endpoints.Customers;

namespace Skyress.API.Endpoints.Customers;

public static class CustomersApiRegistration
{
    public static void MapCustomersApi(this IEndpointRouteBuilder app)
    {
        app.MapCustomersApiV1();
    }

    private static void MapCustomersApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/customers").WithApiVersionSet(versionSet);
        
        // Query endpoints
        api.MapGet("/", GetAllCustomersEndpoint.GetAllCustomersAsync);
        api.MapGet("{id:long}", GetCustomerEndpoint.GetCustomerByIdAsync);
        
        // Command endpoints
        api.MapPost("/", CreateCustomerEndpoint.CreateCustomerAsync);
        api.MapDelete("{id:long}", DeleteCustomerEndpoint.DeleteCustomerAsync);

        // Update endpoints
        api.MapPatch("/state", UpdateCustomerEndpoints.UpdateCustomerStateAsync);
        api.MapPatch("/notes", UpdateCustomerEndpoints.UpdateCustomerNotesAsync);
    }
}