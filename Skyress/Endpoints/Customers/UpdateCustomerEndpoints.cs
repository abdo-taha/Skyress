using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Customers;
using Skyress.Application.Customers.Commands.UpdateCustomerNotes;
using Skyress.Application.Customers.Commands.UpdateCustomerState;
using Skyress.Domain.Aggregates.Customer;

namespace Skyress.API.Endpoints.Customers;

public static class UpdateCustomerEndpoints
{
    public static async Task<Results<Ok<Customer>, NotFound, BadRequest<string>>> UpdateCustomerStateAsync(
        UpdateCustomerStateRequest request,
        ISender sender)
    {
        var command = new UpdateCustomerStateCommand(request.Id, request.State, request.LastEditedBy);
        var result = await sender.Send(command);
        
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Message.Contains("NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }
    
    public static async Task<Results<Ok<Customer>, NotFound, BadRequest<string>>> UpdateCustomerNotesAsync(
        UpdateCustomerNotesRequest request,
        ISender sender)
    {
        var command = new UpdateCustomerNotesCommand(request.Id, request.Notes, request.EditedBy);
        var result = await sender.Send(command);
        
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : result.Error.Message.Contains("NotFound")
                ? TypedResults.NotFound()
                : TypedResults.BadRequest(result.Error.Message);
    }
}