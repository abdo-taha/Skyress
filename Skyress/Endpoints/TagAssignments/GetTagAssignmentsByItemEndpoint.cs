using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.TagAssignments.Queries.GetTagAssignmentsByItem;
using Skyress.Domain.Aggregates.TagAssignmnet;

namespace Skyress.API.Endpoints.TagAssignments;

public static class GetTagAssignmentsByItemEndpoint
{
    public static async Task<Results<Ok<List<TagAssignment>>, BadRequest<string>>> GetTagAssignmentsByItemAsync(
        long itemId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagAssignmentsByItemQuery(itemId), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
}