using System.Reflection;
using Skyress.Domain.Common;
using Skyress.Domain.Exceptions;

namespace Skyress.Application.Abstractions.Messaging;

public static class DomainExceptionResultMapper
{
    public static Result ToFailure(DomainException exception)
    {
        return Result.Failure(ToError(exception));
    }

    public static Result<T> ToFailure<T>(DomainException exception)
    {
        return Result<T>.Failure(ToError(exception));
    }

    public static object ToFailure(Type responseType, DomainException exception)
    {
        var error = ToError(exception);
        if (responseType == typeof(Result))
            return Result.Failure(error);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod(
                nameof(Result<object>.Failure),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                [typeof(Error)]);

            if (failureMethod is not null)
                return failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException($"Cannot map domain exception to response type {responseType.Name}.");
    }

    public static Error ToError(DomainException exception)
    {
        return new Error(exception.Code, exception.Message);
    }
}
