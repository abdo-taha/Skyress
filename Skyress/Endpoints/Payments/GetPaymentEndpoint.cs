using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Payments.Queries.GetPaymentById;
using Skyress.Domain.Common;

namespace Skyress.API.Endpoints.Payments;

public static class GetPaymentEndpoint
{
    public static async Task<Results<Ok<object>, NotFound>> GetPaymentByIdAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(id), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok((object)result.Value);
    }
}