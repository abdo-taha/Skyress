using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.TagAssignments.Commands.DeleteTagAssignment;

namespace Skyress.API.Endpoints.TagAssignments;

public static class DeleteTagAssignmentEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> DeleteTagAssignmentAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTagAssignmentCommand(id), cancellationToken);
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        return TypedResults.Ok();
    }
}