using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Payments;
using Skyress.Application.Payments.Commands.CompleteCashPayment;

namespace Skyress.API.Endpoints.Payments;

public static class CompleteCashPaymentEndpoint
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> CompleteCashPaymentAsync(
        [FromRoute] long id,
        CompletePaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CompleteCashPaymentCommand(id, request.Amount), cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }

        return TypedResults.Ok();
    }
}