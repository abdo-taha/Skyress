using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Customers.Commands.DeleteCustomer;

namespace Skyress.API.Endpoints.Customers;

public static class DeleteCustomerEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> DeleteCustomerAsync(
        long id,
        ISender sender)
    {
        var command = new DeleteCustomerCommand(id);
        var result = await sender.Send(command);
        
        return result.IsSuccess
            ? TypedResults.Ok()
            : result.Error.Message.Contains("NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }
}