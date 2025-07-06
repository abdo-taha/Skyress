using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Queries.GetInvoiceById;
using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.API.Endpoints.Invoices;

public static class GetInvoiceEndpoint
{
    public static async Task<Results<Ok<Invoice>, NotFound>> GetInvoiceByIdAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        if (result.IsFailure)
            return TypedResults.NotFound();
        return TypedResults.Ok(result.Value);
    }
}