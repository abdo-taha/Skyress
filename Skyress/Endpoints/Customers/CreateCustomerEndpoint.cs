using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Customers;
using Skyress.Application.Customers.Commands.CreateCustomer;
using Skyress.Application.Customers.Responses;

namespace Skyress.API.Endpoints.Customers;

public static class CreateCustomerEndpoint
{
    public static async Task<Results<CreatedAtRoute<CustomerResponse>, UnprocessableEntity<ProblemDetails>>> CreateCustomerAsync(
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
            return TypedResults.CreatedAtRoute<CustomerResponse>(
                routeName: nameof(GetCustomerEndpoint),
                routeValues: new { id = result.Value.Id },
                value: result.Value
            );
        }
        else
        {
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }
    }
}
