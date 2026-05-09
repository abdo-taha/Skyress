using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Queries.GetAllInvoices;
using Skyress.Application.Invoices.Responses;

namespace Skyress.API.Endpoints.Invoices;

public static class GetAllInvoicesEndpoint
{
    public static async Task<Ok<IReadOnlyList<InvoiceResponse>>> GetAllInvoicesAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllInvoicesQuery(), cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}