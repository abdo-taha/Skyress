using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Queries.GetInvoicesByCustomer;
using Skyress.Application.Invoices.Responses;

namespace Skyress.API.Endpoints.Invoices;

public static class GetInvoicesByCustomerEndpoint
{
    public static async Task<Ok<IReadOnlyList<InvoiceResponse>>> GetInvoicesByCustomerAsync(
        long customerId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInvoicesByCustomerQuery(customerId), cancellationToken);
        return TypedResults.Ok(result.Value as IReadOnlyList<InvoiceResponse>);
    }
}