using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Customers;
using Skyress.Application.Customers.Commands.UpdateCustomerNotes;
using Skyress.Application.Customers.Commands.UpdateCustomerState;
using Skyress.Application.Customers.Responses;

namespace Skyress.API.Endpoints.Customers;

public static class UpdateCustomerEndpoints
{
    public static async Task<Results<Ok<CustomerResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateCustomerStateAsync(
        UpdateCustomerStateRequest request,
        ISender sender)
    {
        var command = new UpdateCustomerStateCommand(request.Id, request.State);
        var result = await sender.Send(command);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Message.Contains("NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }

    public static async Task<Results<Ok<CustomerResponse>, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateCustomerNotesAsync(
        UpdateCustomerNotesRequest request,
        ISender sender)
    {
        var command = new UpdateCustomerNotesCommand(request.Id, request.Notes);
        var result = await sender.Send(command);

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Message.Contains("NotFound")
                ? TypedResults.NotFound()
                : TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = result.Error.Message,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
    }
}
