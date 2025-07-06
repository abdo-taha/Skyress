using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Payments;
using Skyress.Application.Payments.Commands.CreatePayment;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Common;

namespace Skyress.API.Endpoints.Payments;

public static class CreatePaymentEndpoint
{
    public static async Task<Results<Ok<Payment>, BadRequest<string>, NotFound>> CreatePaymentAsync(
        CreatePaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePaymentCommand(
            request.InvoiceId,
            request.PaymentType),
            cancellationToken);
            
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        
        return TypedResults.Ok(result.Value);
    }
}