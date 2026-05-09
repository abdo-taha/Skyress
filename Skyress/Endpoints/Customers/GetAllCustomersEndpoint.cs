using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Customers.Queries.GetAllCustomers;
using Skyress.Application.Customers.Responses;

namespace Skyress.API.Endpoints.Customers;

public static class GetAllCustomersEndpoint
{
    public static async Task<Ok<IReadOnlyList<CustomerResponse>>> GetAllCustomersAsync(
        ISender sender)
    {
        var query = new GetAllCustomersQuery();
        var result = await sender.Send(query);

        return TypedResults.Ok(result.Value);
    }
}
