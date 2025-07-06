using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Queries.GetInvoiceWithPayments;

namespace Skyress.API.Endpoints.Invoices;

public static class GetInvoiceWithPaymentsEndpoint
{
    public static async Task<Results<Ok<InvoiceWithPaymentsDto>, NotFound>> GetInvoiceWithPaymentsAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInvoiceWithPaymentsQuery(id), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(result.Value);
    }
}