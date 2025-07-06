using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Skyress.API.DTOs.Tags;
using Skyress.Application.Tags.Commands.UpdateTagName;
using Skyress.Application.Tags.Commands.UpdateTagType;

namespace Skyress.API.Endpoints.Tags;

public static class UpdateTagEndpoints
{
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> UpdateTagNameAsync(
        long id,
        UpdateTagNameRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTagNameCommand(id, request.Name), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok();
    }
    
    public static async Task<Results<Ok, NotFound, BadRequest<string>>> UpdateTagTypeAsync(
        long id,
        UpdateTagTypeRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTagTypeCommand(id, request.Type), cancellationToken);
        if (result.IsFailure)
            return TypedResults.BadRequest(result.Error.Message);
        return TypedResults.Ok();
    }
}