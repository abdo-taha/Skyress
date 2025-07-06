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
        
        api.MapGet("/", GetAllPaymentsEndpoint.GetAllPaymentsAsync);
        api.MapGet("{id:long}", GetPaymentEndpoint.GetPaymentByIdAsync);
        api.MapGet("/invoice/{invoiceId:long}", GetPaymentsByInvoiceEndpoint.GetPaymentsByInvoiceAsync);

        api.MapPost("/", CreatePaymentEndpoint.CreatePaymentAsync);
        api.MapDelete("{id:long}", DeletePaymentEndpoint.DeletePaymentAsync);
    }
}