using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Tags.Queries.GetTagsByType;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Tags;

public static class GetTagsByTypeEndpoint
{
    public static async Task<Results<Ok<List<Tag>>, BadRequest<string>>> GetTagsByTypeAsync(
        TagType type,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagsByTypeQuery(type), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok(result.Value);
    }
}