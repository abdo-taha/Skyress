using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Baskets.Queries.GetBasketsByState;
using Skyress.Application.Baskets.Responses;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Baskets;

public static class GetBasketsByStateEndpoint
{
    public static async Task<Results<Ok<IReadOnlyList<BasketResponse>>, UnprocessableEntity<ProblemDetails>>> GetBasketsByStateAsync(
        BasketState state,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBasketsByStateQuery(state), cancellationToken);
        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        return TypedResults.Ok(result.Value);
    }
} 