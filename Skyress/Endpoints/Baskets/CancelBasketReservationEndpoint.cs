using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.CancelBasketReservation;

namespace Skyress.API.Endpoints.Baskets;

public static class CancelBasketReservationEndpoint
{
    public static async Task<Results<Ok, UnprocessableEntity<ProblemDetails>>> CancelBasketReservationAsync(
        ISender sender,
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var command = new CancelBasketReservationCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
    }
} 