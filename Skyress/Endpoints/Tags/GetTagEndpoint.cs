using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Tags.Queries.GetTagById;
using Skyress.Domain.Aggregates.Tag;

namespace Skyress.API.Endpoints.Tags;

public static class GetTagEndpoint
{
    public static async Task<Results<Ok<Tag>, NotFound>> GetTagByIdAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagByIdQuery(id), cancellationToken);
        
        if (result.IsFailure)
            return TypedResults.NotFound();
            
        return TypedResults.Ok(result.Value);
    }
}