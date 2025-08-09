using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.CancelBasketReservation;
using Skyress.API.DTOs.Baskets;

namespace Skyress.API.Endpoints.Baskets;

public static class CancelBasketReservationEndpoint
{
    public static async Task<IResult> CancelBasketReservationAsync(
        ISender sender,
        [FromBody] CancelBasketReservationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CancelBasketReservationCommand(request.BasketId);
        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
} 