using MediatR;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Commands.DeleteBasketCommand;

namespace Skyress.API.Endpoints.Baskets;

public static class DeleteBasketEndpoint
{
    public static async Task<IResult> DeleteBasketAsync(
        ISender sender,
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteBasketCommand(id), cancellationToken);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
    }
    
}