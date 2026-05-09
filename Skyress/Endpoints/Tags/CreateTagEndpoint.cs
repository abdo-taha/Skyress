using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.API.DTOs.Tags;
using Skyress.Application.Tags.Commands.CreateTag;
using Skyress.Application.Tags.Responses;

namespace Skyress.API.Endpoints.Tags;

public static class CreateTagEndpoint
{
    public static async Task<Results<Ok<TagResponse>, UnprocessableEntity<ProblemDetails>>> CreateTagAsync(
        CreateTagRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateTagCommand(request.Name, request.Type), cancellationToken);

        if (result.IsFailure)
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = result.Error.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });

        return TypedResults.Ok(result.Value);
    }
}