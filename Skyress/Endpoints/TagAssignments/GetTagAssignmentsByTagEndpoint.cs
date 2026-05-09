using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByTag;
using Skyress.Application.TagAssignments.Responses;

namespace Skyress.API.Endpoints.TagAssignments;

public static class GetTagAssignmentsByTagEndpoint
{
    public static async Task<Results<Ok<IReadOnlyList<TagAssignmentResponse>>, UnprocessableEntity<ProblemDetails>>> GetTagAssignmentsByTagAsync(
        long tagId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagAssignmentsByTagQuery(tagId), cancellationToken);
        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        return TypedResults.Ok(result.Value as IReadOnlyList<TagAssignmentResponse>);
    }
}