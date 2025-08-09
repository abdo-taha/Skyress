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
        
        // Query endpoints
        api.MapGet("/", GetAllInvoicesEndpoint.GetAllInvoicesAsync);
        api.MapGet("{id:long}", GetInvoiceEndpoint.GetInvoiceByIdAsync);
        api.MapGet("/customer/{customerId:long}", GetInvoicesByCustomerEndpoint.GetInvoicesByCustomerAsync);
        api.MapGet("/state/{state}", GetInvoicesByStateEndpoint.GetInvoicesByStateAsync);
        api.MapGet("{id:long}/withPayments", GetInvoiceWithPaymentsEndpoint.GetInvoiceWithPaymentsAsync);
        
        // Command endpoints
        api.MapDelete("{id:long}", DeleteInvoiceEndpoint.DeleteInvoiceAsync);

        // Update endpoints
        api.MapPatch("{id:long}/customerId", UpdateInvoiceEndpoints.UpdateInvoiceCustomerIdAsync);
        api.MapPatch("{id:long}/state", UpdateInvoiceEndpoints.UpdateInvoiceStateAsync);
    }
}