using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.TagAssignments.Commands.DeleteTagAssignment;

namespace Skyress.API.Endpoints.TagAssignments;

public static class DeleteTagAssignmentEndpoint
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> DeleteTagAssignmentAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTagAssignmentCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }
        return TypedResults.Ok();
    }
}