using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Payments.Queries.GetPaymentsByInvoice;
using Skyress.Domain.Aggregates.Payment;

namespace Skyress.API.Endpoints.Payments;

public static class GetPaymentsByInvoiceEndpoint
{
    public static async Task<Ok<List<Payment>>> GetPaymentsByInvoiceAsync(
        long invoiceId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPaymentsByInvoiceQuery(invoiceId), cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}