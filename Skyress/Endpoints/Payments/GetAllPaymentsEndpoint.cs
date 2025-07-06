using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Payments.Queries.GetAllPayments;
using Skyress.Domain.Aggregates.Payment;

namespace Skyress.API.Endpoints.Payments;

public static class GetAllPaymentsEndpoint
{
    public static async Task<Ok<List<Payment>>> GetAllPaymentsAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllPaymentsQuery(), cancellationToken);
        return TypedResults.Ok(result.Value);
    }
}