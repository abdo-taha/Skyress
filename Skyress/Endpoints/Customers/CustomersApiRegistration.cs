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
        var api = app.MapGroup("api/customers").WithApiVersionSet(versionSet).WithTags("Customers");
        
        // Query endpoints — Admin only
        api.MapGet("/", GetAllCustomersEndpoint.GetAllCustomersAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("{id:long}", GetCustomerEndpoint.GetCustomerByIdAsync).WithName(nameof(GetCustomerEndpoint)).RequireAuthorization(p => p.RequireRole("Admin"));

        // Command endpoints — Admin only
        api.MapPost("/", CreateCustomerEndpoint.CreateCustomerAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapDelete("{id:long}", DeleteCustomerEndpoint.DeleteCustomerAsync).RequireAuthorization(p => p.RequireRole("Admin"));

        // Update endpoints — Admin only
        api.MapPatch("/state", UpdateCustomerEndpoints.UpdateCustomerStateAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("/notes", UpdateCustomerEndpoints.UpdateCustomerNotesAsync).RequireAuthorization(p => p.RequireRole("Admin"));
    }
}