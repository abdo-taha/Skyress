using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Queries.GetAllInvoices;
using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.API.Endpoints.Invoices;

public static class GetAllInvoicesEndpoint
{
    public static async Task<Results<Ok<List<Invoice>>, NotFound>> GetAllInvoicesAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllInvoicesQuery(), cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}