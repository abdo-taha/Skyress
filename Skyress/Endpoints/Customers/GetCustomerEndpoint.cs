using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Customers.Queries.GetCustomer;
using Skyress.Domain.Aggregates.Customer;

namespace Skyress.API.Endpoints.Customers;

public static class GetCustomerEndpoint
{
    public static async Task<Results<Ok<Customer>, NotFound>> GetCustomerByIdAsync(
        long id,
        ISender sender)
    {
        var query = new GetCustomerQuery(id);
        var result = await sender.Send(query);
        
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.NotFound();
    }
}