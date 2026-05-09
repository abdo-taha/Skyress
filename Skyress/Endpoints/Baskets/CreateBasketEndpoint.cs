using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Baskets;
using Skyress.Application.Baskets.Commands.CreateBasket;
using Skyress.Application.Baskets.Responses;

namespace Skyress.API.Endpoints.Baskets;

public static class CreateBasketEndpoint
{
    public static async Task<Results<Ok<BasketResponse>, UnprocessableEntity<ProblemDetails>>> CreateBasketAsync(
        ISender sender,
        [FromBody] CreateBasketRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateBasketCommand(request.CustomerId), cancellationToken);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
    }
}