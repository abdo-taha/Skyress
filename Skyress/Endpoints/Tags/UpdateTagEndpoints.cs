using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Tags;
using Skyress.Application.Tags.Commands.UpdateTagName;
using Skyress.Application.Tags.Commands.UpdateTagType;

namespace Skyress.API.Endpoints.Tags;

public static class UpdateTagEndpoints
{
    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateTagNameAsync(
        long id,
        UpdateTagNameRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTagNameCommand(id, request.Name), cancellationToken);
        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>>> UpdateTagTypeAsync(
        long id,
        UpdateTagTypeRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateTagTypeCommand(id, request.Type), cancellationToken);
        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        return TypedResults.Ok();
    }
}