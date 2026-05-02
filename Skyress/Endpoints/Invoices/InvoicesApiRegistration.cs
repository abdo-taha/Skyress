using Asp.Versioning.Conventions;

namespace Skyress.API.Endpoints.Invoices;

public static class InvoicesApiRegistration
{
    public static void MapInvoicesApi(this IEndpointRouteBuilder app)
    {
        app.MapInvoicesApiV1();
    }

    private static void MapInvoicesApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/invoices").WithApiVersionSet(versionSet).WithTags("Invoices");
        
        // Query endpoints — Admin only
        api.MapGet("/", GetAllInvoicesEndpoint.GetAllInvoicesAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("{id:long}", GetInvoiceEndpoint.GetInvoiceByIdAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("/customer/{customerId:long}", GetInvoicesByCustomerEndpoint.GetInvoicesByCustomerAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("/state/{state}", GetInvoicesByStateEndpoint.GetInvoicesByStateAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("{id:long}/withPayments", GetInvoiceWithPaymentsEndpoint.GetInvoiceWithPaymentsAsync).RequireAuthorization(p => p.RequireRole("Admin"));

        // Command endpoints — Admin only
        api.MapDelete("{id:long}", DeleteInvoiceEndpoint.DeleteInvoiceAsync).RequireAuthorization(p => p.RequireRole("Admin"));

        // Update endpoints — Admin only
        api.MapPatch("{id:long}/customerId", UpdateInvoiceEndpoints.UpdateInvoiceCustomerIdAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapPatch("{id:long}/state", UpdateInvoiceEndpoints.UpdateInvoiceStateAsync).RequireAuthorization(p => p.RequireRole("Admin"));
    }
}