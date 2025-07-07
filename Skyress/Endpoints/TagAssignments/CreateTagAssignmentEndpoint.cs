using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.TagAssignments;
using Skyress.Application.TagAssignments.Commands.CreateTagAssignment;
using Skyress.Domain.Aggregates.TagAssignmnet;

namespace Skyress.API.Endpoints.TagAssignments;

public static class CreateTagAssignmentEndpoint
{
    public static async Task<Results<Ok<TagAssignment>, BadRequest<string>>> CreateTagAssignmentAsync(
        CreateTagAssignmentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTagAssignmentCommand(request.TagId, request.ItemId), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
}