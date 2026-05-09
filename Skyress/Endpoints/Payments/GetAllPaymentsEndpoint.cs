using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Payments.Queries.GetAllPayments;
using Skyress.Application.Payments.Responses;

namespace Skyress.API.Endpoints.Payments;

public static class GetAllPaymentsEndpoint
{
    public static async Task<Ok<IReadOnlyList<PaymentResponse>>> GetAllPaymentsAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllPaymentsQuery(), cancellationToken);
        return TypedResults.Ok(result.Value as IReadOnlyList<PaymentResponse>);
    }
}