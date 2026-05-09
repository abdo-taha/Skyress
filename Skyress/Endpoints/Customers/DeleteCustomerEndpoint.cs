using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Customers.Commands.DeleteCustomer;

namespace Skyress.API.Endpoints.Customers;

public static class DeleteCustomerEndpoint
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> DeleteCustomerAsync(
        long id,
        ISender sender)
    {
        var command = new DeleteCustomerCommand(id);
        var result = await sender.Send(command);

        return result.IsSuccess
            ? TypedResults.Ok()
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
