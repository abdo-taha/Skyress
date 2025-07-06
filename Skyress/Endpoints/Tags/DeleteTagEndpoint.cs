using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.Application.Tags.Commands.DeleteTag;

namespace Skyress.API.Endpoints.Tags;

public static class DeleteTagEndpoint
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> DeleteTagAsync(
        long id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTagCommand(id), cancellationToken);
        
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.Error.Message);
        }
        
        return TypedResults.Ok();
    }
}