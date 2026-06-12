using FluentValidation;
using MediatR;
using Skyress.Application.Abstractions.Messaging;
using Skyress.Domain.Exceptions;

namespace Skyress.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IBaseCommand || !validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => failure.ErrorMessage)
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
            return await next();

        var exception = new DomainException("Validation.Failed", string.Join("; ", errors));
        return (TResponse)DomainExceptionResultMapper.ToFailure(typeof(TResponse), exception);
    }
}
