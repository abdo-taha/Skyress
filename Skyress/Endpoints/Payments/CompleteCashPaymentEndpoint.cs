using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Payments.Commands.CompleteCashPayment;

namespace Skyress.API.Endpoints.Payments;

public static class CompleteCashPaymentEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> CompleteCashPaymentAsync(
        [FromRoute] long id,
        [FromBody] decimal amount,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CompleteCashPaymentCommand(id, amount), cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }

        return TypedResults.Ok();
    }
}