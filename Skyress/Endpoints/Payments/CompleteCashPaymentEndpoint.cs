using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Payments;
using Skyress.Application.Payments.Commands.CompleteCashPayment;

namespace Skyress.API.Endpoints.Payments;

public static class CompleteCashPaymentEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> CompleteCashPaymentAsync(
        [FromRoute] long id,
        CompletePaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CompleteCashPaymentCommand(id, request.Amount), cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }

        return TypedResults.Ok();
    }
}