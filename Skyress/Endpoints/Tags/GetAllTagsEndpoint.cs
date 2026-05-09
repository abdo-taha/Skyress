using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Tags.Queries.GetAllTags;
using Skyress.Application.Tags.Responses;

namespace Skyress.API.Endpoints.Tags;

public static class GetAllTagsEndpoint
{
    public static async Task<Ok<IReadOnlyList<TagResponse>>> GetAllTagsAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllTagsQuery(), cancellationToken);
        return TypedResults.Ok(result.Value as IReadOnlyList<TagResponse>);
    }
}