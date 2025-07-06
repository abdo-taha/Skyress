using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Tags;
using Skyress.Application.Tags.Commands.CreateTag;
using Skyress.Domain.Aggregates.Tag;

namespace Skyress.API.Endpoints.Tags;

public static class CreateTagEndpoint
{
    public static async Task<Results<Ok<Tag>, BadRequest<string>>> CreateTagAsync(
        CreateTagRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTagCommand(request.Name, request.Type), cancellationToken);
        
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
            
        return TypedResults.Ok(result.Value);
    }
}