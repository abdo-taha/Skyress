using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Application.Tags.Queries.GetTagsByType;
using Skyress.Application.Tags.Responses;
using Skyress.Domain.Enums;

namespace Skyress.API.Endpoints.Tags;

public static class GetTagsByTypeEndpoint
{
    public static async Task<Results<Ok<IReadOnlyList<TagResponse>>, UnprocessableEntity<ProblemDetails>>> GetTagsByTypeAsync(
        TagType type,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagsByTypeQuery(type), cancellationToken);
        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        return TypedResults.Ok(result.Value as IReadOnlyList<TagResponse>);
    }
}