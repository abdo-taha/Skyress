using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Invoices;
using Skyress.Application.Invoices.Commands.UpdateInvoiceCustomerId;
using Skyress.Application.Invoices.Commands.UpdateInvoiceState;
using Skyress.Application.Invoices.Responses;

namespace Skyress.API.Endpoints.Invoices;

public static class UpdateInvoiceEndpoints
{
    public static async Task<Results<Ok<InvoiceResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateInvoiceCustomerIdAsync(
        long id,
        UpdateInvoiceCustomerIdRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateInvoiceCustomerIdCommand(id, request.CustomerId), cancellationToken);
        if (result.IsFailure)
            return result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
        return TypedResults.Ok(result.Value);
    }

    public static async Task<Results<Ok<InvoiceResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateInvoiceStateAsync(
        long id,
        UpdateInvoiceStateRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateInvoiceStateCommand(id, request.State), cancellationToken);
        if (result.IsFailure)
            return result.Error.Code.EndsWith(".NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
        return TypedResults.Ok(result.Value);
    }
}