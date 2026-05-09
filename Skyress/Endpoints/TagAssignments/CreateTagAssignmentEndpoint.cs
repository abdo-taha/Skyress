using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.TagAssignments;
using Skyress.Application.TagAssignments.Commands.CreateTagAssignment;
using Skyress.Application.TagAssignments.Responses;

namespace Skyress.API.Endpoints.TagAssignments;

public static class CreateTagAssignmentEndpoint
{
    public static async Task<Results<Ok<TagAssignmentResponse>, UnprocessableEntity<ProblemDetails>>> CreateTagAssignmentAsync(
        CreateTagAssignmentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTagAssignmentCommand(request.TagId, request.ItemId), cancellationToken);
        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        return TypedResults.Ok(result.Value);
    }
}