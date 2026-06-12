using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Skyress.Domain.Common;

namespace Skyress.API.Endpoints;

public static class EndpointResultExtensions
{
    public static ProblemDetails ToProblemDetails(
        this Error error,
        int statusCode = StatusCodes.Status422UnprocessableEntity)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Business Rule Violation",
            Detail = error.Message,
            Status = statusCode
        };
        problemDetails.Extensions["code"] = error.Code;
        return problemDetails;
    }

    public static Results<Ok<T>, UnprocessableEntity<ProblemDetails>> ToOkOrUnprocessableEntity<T>(
        this Result<T> result)
    {
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.UnprocessableEntity(result.Error.ToProblemDetails());
    }

    public static Results<Ok<T>, NotFound, UnprocessableEntity<ProblemDetails>> ToOkOrNotFoundOrUnprocessableEntity<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error.Code.EndsWith(".NotFound", StringComparison.Ordinal)
            ? TypedResults.NotFound()
            : TypedResults.UnprocessableEntity(result.Error.ToProblemDetails());
    }

    public static Results<Ok, UnprocessableEntity<ProblemDetails>> ToOkOrUnprocessableEntity(
        this Result result)
    {
        return result.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.UnprocessableEntity(result.Error.ToProblemDetails());
    }

    public static Results<Ok, NotFound, UnprocessableEntity<ProblemDetails>> ToOkOrNotFoundOrUnprocessableEntity(
        this Result result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok();

        return result.Error.Code.EndsWith(".NotFound", StringComparison.Ordinal)
            ? TypedResults.NotFound()
            : TypedResults.UnprocessableEntity(result.Error.ToProblemDetails());
    }
}
