using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByTag;
using Skyress.Domain.Aggregates.TagAssignmnet;

namespace Skyress.API.Endpoints.TagAssignments;

public static class GetTagAssignmentsByTagEndpoint
{
    public static async Task<Results<Ok<List<TagAssignment>>, BadRequest<string>>> GetTagAssignmentsByTagAsync(
        long tagId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagAssignmentsByTagQuery(tagId), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
}