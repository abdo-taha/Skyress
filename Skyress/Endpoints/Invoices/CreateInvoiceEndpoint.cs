using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Invoices;
using Skyress.Application.Invoices.Commands.CreateInvoice;
using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.API.Endpoints.Invoices;

public static class CreateInvoiceEndpoint
{
    public static async Task<Results<Ok<Invoice>, BadRequest<string>>> CreateInvoiceAsync(
        CreateInvoiceRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateInvoiceCommand(request.CustomerId, request.TotalAmount), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
}