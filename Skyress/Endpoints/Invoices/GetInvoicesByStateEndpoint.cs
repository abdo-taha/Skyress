using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Queries.GetInvoicesByState;
using Skyress.Application.Invoices.Responses;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Invoices;

public static class GetInvoicesByStateEndpoint
{
    public static async Task<Ok<IReadOnlyList<InvoiceResponse>>> GetInvoicesByStateAsync(
        InvoiceState state,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInvoicesByStateQuery(state), cancellationToken);
        return TypedResults.Ok(result.Value as IReadOnlyList<InvoiceResponse>);
    }
}