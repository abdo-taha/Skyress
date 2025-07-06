using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Invoices.Commands.DeleteInvoice;

namespace Skyress.API.Endpoints.Invoices;

public static class DeleteInvoiceEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> DeleteInvoiceAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteInvoiceCommand(id), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok();
    }
}