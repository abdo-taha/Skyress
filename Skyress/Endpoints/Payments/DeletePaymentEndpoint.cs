using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Payments.Commands.DeletePayment;
using Skyress.Domain.Common;

namespace Skyress.API.Endpoints.Payments;

public static class DeletePaymentEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> DeletePaymentAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeletePaymentCommand(id), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        
        return TypedResults.Ok();
    }
}