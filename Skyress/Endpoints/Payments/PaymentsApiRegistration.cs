using Asp.Versioning.Conventions;

namespace Skyress.API.Endpoints.Payments;

public static class PaymentsApiRegistration
{
    public static void MapPaymentsApi(this IEndpointRouteBuilder app)
    {
        app.MapPaymentsApiV1();
    }

    private static void MapPaymentsApiV1(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
        var api = app.MapGroup("api/payments").WithApiVersionSet(versionSet).WithTags("Payments");
        
        // All payment endpoints — Admin only
        api.MapGet("/", GetAllPaymentsEndpoint.GetAllPaymentsAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("{id:long}", GetPaymentEndpoint.GetPaymentByIdAsync).RequireAuthorization(p => p.RequireRole("Admin"));
        api.MapGet("/invoice/{invoiceId:long}", GetPaymentsByInvoiceEndpoint.GetPaymentsByInvoiceAsync).RequireAuthorization(p => p.RequireRole("Admin"));

        api.MapPost("{id:long}/Pay", CompleteCashPaymentEndpoint.CompleteCashPaymentAsync).RequireAuthorization(p => p.RequireRole("Admin"));
    }
}