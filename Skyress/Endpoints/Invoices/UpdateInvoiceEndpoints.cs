using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Invoices;
using Skyress.Application.Invoices.Commands.UpdateInvoiceCustomerId;
using Skyress.Application.Invoices.Commands.UpdateInvoiceState;
using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.API.Endpoints.Invoices;

public static class UpdateInvoiceEndpoints
{
    public static async Task<Results<Ok<Invoice>, NotFound, BadRequest<string>>> UpdateInvoiceCustomerIdAsync(
        long id,
        UpdateInvoiceCustomerIdRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateInvoiceCustomerIdCommand(id, request.CustomerId), cancellationToken);
        if (result.IsFailure)
            return result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
    
    public static async Task<Results<Ok<Invoice>, NotFound, BadRequest<string>>> UpdateInvoiceStateAsync(
        long id,
        UpdateInvoiceStateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateInvoiceStateCommand(id, request.State), cancellationToken);
        if (result.IsFailure)
            return result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
}