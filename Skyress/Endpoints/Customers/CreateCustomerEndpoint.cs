using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Customers;
using Skyress.Application.Customers.Commands.CreateCustomer;
using Skyress.Domain.Aggregates.Customer;

namespace Skyress.API.Endpoints.Customers;

public static class CreateCustomerEndpoint
{
    public static async Task<Results<Created<Customer>, BadRequest<string>>> CreateCustomerAsync(
        CreateCustomerRequest request,
        HttpContext httpContext,
        ISender sender)
    {
        var command = new CreateCustomerCommand(
            request.Name,
            request.Notes,
            request.State
        );
        
        var result = await sender.Send(command);
        
        if (result.IsSuccess)
        {
            return TypedResults.Created(result.Value.Name, result.Value);
        }
        else
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
    }
}