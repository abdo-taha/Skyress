using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Tags.Commands.DeleteTag;

namespace Skyress.API.Endpoints.Tags;

public static class DeleteTagEndpoint
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> DeleteTagAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTagCommand(id), cancellationToken);

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